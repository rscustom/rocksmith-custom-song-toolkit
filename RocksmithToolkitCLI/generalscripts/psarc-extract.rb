require 'openssl'
require 'zlib'
require 'fileutils'
require 'digest'

class PSARC
  def initialize(fn)
    @fn = fn
    @psarc = File.open(@fn, 'rb')

    # PSARC header
    magic = read(0, 4); throw 'wrong magic' if magic != 'PSAR'
    #psarc[4...8] = ? 00 00 00 1E
    algo = read(8, 12); throw 'wrong compression' if algo != 'zlib'
    # end of TOC, start of data
    @zsize = read(12, 16).unpack('N')[0]
    #psarc[16...20] = ? = 
    @numentries = read(20, 24).unpack('N')[0]
    @blksize = read(24, 28).unpack('N')[0]
    #psarc[28...32] = ? = 00 00 00 04
    @toc = decrypt_toc(read(32, @zsize))

    read_blocks
    read_entries
    @filenames = ['NamesBlock.bin']
    @filenames.concat extract_entry(0).split("\n")
  end

  def open_path(out)
    FileUtils.mkdir_p(File.dirname(out))
    f = File.open(out, 'wb')
    return f
  end

  def extract(filter=nil)
    filter = Regexp.new(filter) if filter
    @numentries.times do |i|
      next if filter && @filenames[i] !~ filter
      data = extract_entry(i)
      f = open_path(@fn + '.extracted/' + @filenames[i])
      f.write(data)
      f.close
    end
  end

  def read_blocks()
    @block = []
    entrysize = 30
    # 2 because blocksize = 64K
    numblocks = (@zsize - 32 - @numentries*entrysize) / 2
    numblocks.times do |n|
      offset = @numentries*entrysize + (2*n)
      size = @toc[offset...offset+2].unpack('n')[0]
      @block[n] = size
    end
  end

  def pack_n(data, n)
    n = data.bytesize if n == nil
    value = 0
    0.upto(n-1) do |i|
      value += (data[i] << 8*(n-1-i))
    end
    return value
  end

  def read_entries()
    @entry = []
    @numentries.times do |n|
      # MD5 of filename
      offset = (n*30)
      md5 = @toc[offset...offset+16].unpack('a*')[0]
      # index to block table
      offset += 16
      blockn = @toc[offset...offset+4].unpack('N')[0]
      # unpacked length
      offset += 4
      data = @toc[offset...offset+5].unpack('C*')
      zlen = pack_n(data, 5)
      # offset in PSARC
      offset += 5
      data = @toc[offset...offset+5].unpack('C*')
      zoff = pack_n(data, 5)

      @entry[n] = {
        'block' => blockn,
        'zoff' => zoff,
        'zlen' => zlen,
        'md5' => md5,
      }
    end
  end

  def extract_entry(i)
    puts @entry[i]
    puts @filenames[i]
    entry = @entry[i]
    offset = entry['zoff']
    zindex = entry['block']

    file = ''
    # consume and decompress blocks until we reach length
    begin
      break if entry['zlen'] == 0
      size = @block[zindex]
      data = nil

      if size == 0
        size = @blksize
        data = read(offset, offset+size)
        # uncompressed
      else
        data = read(offset, offset+size)
        if 0x78DA == data[0...2].unpack('n')[0]
          # every block needs new initialization
          inf = Zlib::Inflate.new()
          data = inf.inflate(data)
          puts "decompressed block #{data.bytesize}"
        end
      end

      file << data
      offset += size
      zindex += 1
    end while file.bytesize < entry['zlen']

    if file.bytesize != entry['zlen']
      throw "expected size #{entry['zlen']}, got #{file.bytesize}"
    end

    if i != 0
      # check MD5 of filename
      md5 = Digest::MD5.digest(@filenames[i])
      if md5 != entry['md5']
        throw "expected filename MD5 #{entry['md5']}, got #{md5}"
      end
    end

    return file
  end

  def decrypt_toc(enctoc)
    return enctoc
  end

  def fread(fn)
    return File.open(fn, 'rb').read()  
  end

  # def read(offset, stop)
  #   return @psarc[offset...stop]
  # end

  def read(offset, stop)
    @psarc.seek(offset)
    return @psarc.read(stop - offset)
  end

  def print_b(str, length=nil)
    if not length
      str.each_byte { |x| printf "%02x ", x }
    else
      str[0...length].each_byte { |x| printf "%02x ", x }
    end
    puts
  end
end

class PSARC_RS2014 < PSARC
  def self.convert_key(key)
    keystr = ''
    key.each { |k|
      a = k >> 24
      b = k >> 16 & 0xff
      c = k >> 8 & 0xff
      d = k & 0xff
      dword = sprintf('%c%c%c%c', d,c,b,a)
      #printf "0x%02x, ", d
      #printf "0x%02x, ", c
      #printf "0x%02x, ", b
      #printf "0x%02x, ", a
      keystr += dword
    }
    return keystr
  end

  # AES-256-CFB, uninitialized IV
  # TOC header key
  @@tockey = PSARC_RS2014.convert_key(
    [ 0x38b23dc5, 0xf7a2a170, 0x0664ae1c, 0x110edd1f,
      0xc89d3057, 0xc5d40452, 0x0925dfbf, 0x2c57f20d ]
  )

  def decrypt_toc(enctoc)
    return AESCrypt.decrypt(enctoc, @@tockey, nil, 'aes-256-cfb')
  end

  def fix_toc()
    # only fix TOC to be able to use other tools
    @toc.length.times { |i| @psarc[32+i] = @toc[i] }
    File.open(@fn + '.decrypted.psarc', 'wb').write(@psarc)
  end
end

module AESCrypt
  # Decrypts a block of data (encrypted_data) given an encryption key
  # and an initialization vector (iv).  Keys, iv's, and the data
  # returned are all binary strings.  Cipher_type should be
  # "AES-256-CBC", "AES-256-ECB", or any of the cipher types
  # supported by OpenSSL.  Pass nil for the iv if the encryption type
  # doesn't use iv's (like ECB).
  #:return: => String
  #:arg: encrypted_data => String
  #:arg: key => String
  #:arg: iv => String
  #:arg: cipher_type => String
  def AESCrypt.decrypt(encrypted_data, key, iv=nil, cipher_type="aes-256-cfb")
    aes = OpenSSL::Cipher::Cipher.new(cipher_type)
    aes.decrypt
    aes.key = key
    #aes.iv = iv if iv != nil
    #aes.iv = key[16...32]
    aes.update(encrypted_data) + aes.final
  end

  # Encrypts a block of data given an encryption key and an
  # initialization vector (iv).  Keys, iv's, and the data returned
  # are all binary strings.  Cipher_type should be "AES-256-CBC",
  # "AES-256-ECB", or any of the cipher types supported by OpenSSL.
  # Pass nil for the iv if the encryption type doesn't use iv's (like
  # ECB).
  #:return: => String
  #:arg: data => String
  #:arg: key => String
  #:arg: iv => String
  #:arg: cipher_type => String
  def AESCrypt.encrypt(data, key, iv=nil, cipher_type="aes-256-cbc")
    aes = OpenSSL::Cipher::Cipher.new(cipher_type)
    aes.encrypt
    aes.key = key
    aes.iv = iv if iv != nil
    aes.update(data) + aes.final
  end
end

fn = ARGV[0]
p = PSARC_RS2014.new(fn)
filter = ARGV[1]
p.extract(filter)

#p.fix_toc()
#p = PSARC.new(fn + '.decrypted.psarc')
#p.extract(filter)

###
# 0-4   magic
# 4-8   00 01 00 04 = 65540
# 8-12  zlib
# 12-16 zSize 00 00 03 B4 = 948 offset of file list
# 16-20 00 00 00 1E = 30?
# 20-24 numentries = 00 00 00 17 = 23
# 24-28 blocksize = 00 01 00 00 = 64k
# 28-32 00 00 00 04
# 32-36 4E 3A 0A 91 start of TOC (to 948 = 3B4)
#  16 MD5
#  4 = 32 zindex
#  5 = 40 length
#  5 = 40 zoffset
#  = 30B entry * numentries
#  + 2/3/4 * numblocks => 2*numblocks because 256*256 >= blocksize
#  numBlocks = (zSize - (uint32_t)_f.offset()) / zType;
#  numBlocks = (948 - 32 - 23*30) / 2 = 113 x WORD value (size of block)
# TO EXTRACT: go through blocks until you hit length
# (original)
