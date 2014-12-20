if ARGV.length != 1
  puts "usage: #{$0} <HSL file>"
  exit(1)
end

input = ARGV[0]

def capitalize(name)
  return name if name =~ /[a-z]/
  name.split('_').map(&:capitalize).join('')
end

def get_name(name)
  name.sub(/\[.*\]/, '')
end

def n(lvl=0)
  return "\n"+t(lvl)
end

def t(lvl=0)
  return "\t"*lvl
end

puts <<EOF
// AUTO-GENERATED FILE, DO NOT MODIFY!
using System;
using System.IO;
using MiscUtil.IO;

namespace RocksmithToolkitLib.Sng2014HSL {
EOF

order = []
readfn = {
  'Byte' => 'r.ReadByte()',
  'Int16' => 'r.ReadInt16()',
  'Int32' => 'r.ReadInt32()',
  'UInt32' => 'r.ReadUInt32()',
  'float' => 'r.ReadSingle()',
  'double' => 'r.ReadDouble()',
}

ind = n(2) #8 spaces
reader = []
File.open(input).readlines.each do |l|
  break if l =~ /function ParseFile/
  next if l =~ /^#/         #skip #include tags
  next if l =~ /^\s*\/\//   #skip comments
  next if l =~ /^\s$/       #skip white space
  next if l =~ /^{$/        #skip redundant '{'
#struct block
  if l =~ /^struct (.*)/
    name = capitalize($1)
    puts "\tpublic class #{name} {"
    next
  end
#inner struct, custom type
  if l =~ /(\s+)struct (.+)\s+(.*);/
    type = capitalize($2)
    l = "#{t(2)}public #{type} #{$3} { get; set; }"
    name = get_name($3)
    order << name
    reader << "#{name}.read(r);"
    # need to construct object too
    # will be rewritten for arrays
    reader[-1] = "#{name} = new #{type}(); " + reader[-1];
#common types
  elsif l =~ /^(.+)ulong (.*);/
    l = "#{t(2)}public UInt32 #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "#{name} = r.ReadUInt32();"
  elsif l =~ /^(.+)long (.*);/
    l = "#{t(2)}public Int32 #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "#{name} = r.ReadInt32();"
  elsif l =~ /^(.+)short (.*);/
    l = "#{t(2)}public Int16 #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "#{name} = r.ReadInt16();"
  elsif l =~ /^(.+)char (.*);/ || l =~ /^(.+)byte (.*);/
    l = "#{t(2)}public Byte #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "#{name} = r.ReadByte();"
  elsif l =~ /^(.+)float (.*);/
    l = "#{t(2)}public float #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "#{name} = r.ReadSingle();"
  elsif l =~ /^(.+)double (.*);/
    l = "#{t(2)}public double #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "#{name} = r.ReadDouble();"
  elsif l =~ /^(\s+)(.+) (.*);/
    throw "Unhandled type: #{$2}"
  end

  if l =~ /^};$/    #order[]
    list = order.map { |name| "\t\"#{name}\"" }.join(','+ind)
    puts "#{ind}public string[] _order = {#{ind}#{list}#{ind}};"
    puts "\t\tpublic string[] order { get { return _order; } }"
    puts "#{t(2)}public void read(EndianBinaryReader r) {#{n(3)}#{reader.join(n(3))}#{ind}}"
    puts "\t}\n"
    order = []
    reader = []
    next
  end

  if l =~ /(\s*)public\s+(.+)\s+(.+)\[(.+)\](.*)/    #read()
    ws, type, count, name, rest = $1, $2, $4, $3, $5
    prefix = "this."
    if count =~ /^[0-9]+$/
      l = "#{ws}public #{type}[] _#{name} = new #{type}[#{count}];\n"
      l << "#{ws}public #{type}[] #{name} { get { return _#{name}; } set { _#{name} = value; } }"
      prefix = ''
    else
      l = "#{ws}public #{type}[] #{name}#{rest}"
    end
    if type == 'Byte'
      reader[-1] = "#{name} = r.ReadBytes(#{count});"
    elsif readfn.include? type
      reader[-1] = "#{name} = new #{type}[#{count}]; for (int i = 0; i < #{count}; i++) #{name}[i] = #{readfn[type]};"
    else
      reader[-1] = "#{name} = new #{type}[#{count}]; for (int i = 0; i < #{count}; i++) { var obj = new #{type}(); obj.read(r); #{name}[i] = obj; }"
    end
  end
  puts l if l
end

puts "}"
