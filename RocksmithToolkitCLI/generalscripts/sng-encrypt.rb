require 'openssl'
require 'zlib'

def convert_key(key)
  keystr = ''
  key.each { |k|
    a = k >> 24
    b = k >> 16 & 0xff
    c = k >> 8 & 0xff
    d = k & 0xff
    dword = sprintf('%c%c%c%c', d,c,b,a)
    keystr += dword
  }
  return keystr
end

# AES-256-CFB, uninitialized IV
# TOC header key
toc = [ 0x38b23dc5,	  0xf7a2a170,	0x0664ae1c,	0x110edd1f,
        0xc89d3057,	  0xc5d40452,	0x0925dfbf,	0x2c57f20d ]
$tockey = convert_key(toc)

# AES-256-CTR, 16 bytes of IV comes from data
# SNG key - Mac
sngmac = [0x0e332198, 0x701fb934, 0xbd8ca4d0, 0x12935962,
          0xa0ce7069, 0xe6c09291, 0xcc76a6cd, 0x9d283898]
$sngmackey = convert_key(sngmac)
$sngwinkey = "\xCB\x64\x8D\xF3\xD1\x2A\x16\xBF\x71\x70\x14\x14\xE6\x96\x19\xEC\x17\x1C\xCA\x5D\x2A\x14\x2E\x3E\x59\xDE\x7A\xDD\xA1\x8A\x3A\x30"

$key = {
  'xbox' => nil,
  'mac' => $sngmackey,
  'win' => $sngwinkey,
}

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
    aes.iv = iv if iv != nil
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
  def AESCrypt.encrypt(data, key, iv=nil, cipher_type="aes-256-cfb")
    aes = OpenSSL::Cipher::Cipher.new(cipher_type)
    aes.encrypt
    aes.key = key
    aes.iv = iv if iv != nil
    aes.update(data) + aes.final
  end
end

def fread(fn)
  return File.open(fn, 'rb').read()  
end

def print_b(str, length=nil)
  if not length
    str.bytes.to_a.each { |x| printf "%02x ", x }
  else
    str.bytes.to_a[0...length].each { |x| printf "%02x ", x }
  end
  puts
end

def get_sng_data(data, platform)
  payload = data[8...data.bytesize]
  iv = payload[0...16]
  cipher = payload[16...data.bytesize]
  if $key[platform]
    decrypted = AESCrypt.encrypt(cipher, $key[platform], iv, 'aes-256-ctr')
  else
    decrypted = payload
  end
  zblock = decrypted[4...decrypted.bytesize]

  inf = Zlib::Inflate.new()
  plain = inf.inflate(decrypted[4...decrypted.bytesize])

  if platform == 'win' or platform == 'mac'
    size = decrypted[0...4].unpack('L')[0]
  else
    size = decrypted[0...4].unpack('N')[0]
  end

  if (plain.bytesize != size)
    throw "expected size #{size}, got #{plain.bytesize}"
  end

  return [payload, iv, cipher, decrypted, plain, zblock]
end

def pack_sng(fn, platform, output=nil)
  data = fread(fn)
  payload, iv, cipher, decrypted, plain, zblock = get_sng_data(data, platform)

  out = fn + '.sng'
  out = output + File.basename(fn) + '.sng' if output
  File.open(out, 'wb') {|f| f.write(plain)}

  #puts "DECRYPTED"
  #print_b(decrypted, 32)

  # puts "ENCRYPTED SIGNATURE 56"
  # (data.bytesize-56).upto(data.bytesize-1).each { |x| printf "%02x ", data[x].ord }
  # puts "\nPLAIN SIGNATURE 56"
  # (decrypted.bytesize-56).upto(decrypted.bytesize-1).each { |x| printf "%02x ", decrypted[x].ord }
  # puts
end

if ARGV.length != 2 && ARGV.length != 3
  puts "usage: #{$0} <mac/win/xbox> <sng file> [output directory]"
  exit(1)
end

platform = ARGV[0]
input = ARGV[1]
output = ARGV[2]
#pack_sng(input, platform, output)

if ARGV.length != 2
  puts "usage: #{$0} <mac/win/xbox> <sng file>"
  exit(1)
end

# test mode - read real SNG file and produce identical copy
#data = fread(input)
#payload, iv, cipher, decrypted, plain, zblock = get_sng_data(data, platform)
#sngdata = plain
#signature = data[-56..-1]

# raw SNG data
sngdata = fread(input)
iv = "\x00" * 16
signature = "\x00" * 56

# packing
size = [sngdata.bytesize].pack('L')
packed = Zlib::Deflate.deflate(sngdata, Zlib::BEST_COMPRESSION)
plain = size + packed
enc = AESCrypt.encrypt(plain, $key[platform], iv, 'aes-256-ctr')

header = [0x4A, 0x03].pack('L*')
f = File.open(input + '.packed.sng', 'wb')
f.write(header)
f.write(iv)
f.write(enc)
f.write(signature)
f.close
