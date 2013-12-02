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

puts <<EOF
// AUTO-GENERATED FILE, DO NOT MODIFY!
using System;
using System.IO;
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
reader = []
File.open(input).readlines.each do |l|
  break if l =~ /function ParseFile/
  next if l =~ /^#/
  next if l =~ /^\s*\/\//
  if l =~ /^};$/
    list = order.map { |name| "    \"#{name}\"" }.join(",\n    ")
    puts "\n    public string[] _order = {\n    #{list}\n" + (' '*4) + '};'
    puts "    public string[] order { get { return this._order; } }"
    puts "    public void read(BinaryReader r) {\n        #{reader.join("\n" + ' '*8)}\n    }"
    puts "}\n"
    order = []
    reader = []
    next
  end
  if l =~ /^struct (.*)/
    name = capitalize($1)
    puts "public class #{name}"
    next
  end

  if l =~ /(\s+)struct (.+)\s+(.*);/
    type = capitalize($2)
    l = "#{$1}public #{type} #{$3} { get; set; }"
    name = get_name($3)
    order << name
    reader << "this.#{name}.read(r);"
    # need to construct object too
    # will be rewritten for arrays
    reader[-1] = "this.#{name} = new #{type}(); " + reader[-1];
  elsif l =~ /^(.+)ulong (.*);/
    l = "#{$1}public UInt32 #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "this.#{name} = r.ReadUInt32();"
  elsif l =~ /^(.+)long (.*);/
    l = "#{$1}public Int32 #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "this.#{name} = r.ReadInt32();"
  elsif l =~ /^(.+)short (.*);/
    l = "#{$1}public Int16 #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "this.#{name} = r.ReadInt16();"
  elsif l =~ /^(.+)char (.*);/ || l =~ /^(.+)byte (.*);/
    l = "#{$1}public Byte #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "this.#{name} = r.ReadByte();"
  elsif l =~ /^(.+)float (.*);/
    l = "#{$1}public float #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "this.#{name} = r.ReadSingle();"
  elsif l =~ /^(.+)double (.*);/
    l = "#{$1}public double #{$2} { get; set; }"
    name = get_name($2)
    order << name
    reader << "this.#{name} = r.ReadDouble();"
  elsif l =~ /^(\s+)(.+) (.*);/
    throw "Unhandled type: #{$2}"
  end

  if l =~ /(\s*)public\s+(.+)\s+(.+)\[(.+)\](.*)/
    ws, type, count, name, rest = $1, $2, $4, $3, $5
    prefix = "this."
    if count =~ /^[0-9]+$/
      l = "#{ws}public #{type}[] _#{name} = new #{type}[#{count}];\n"
      l << "#{ws}public #{type}[] #{name} { get { return this._#{name}; } set { _#{name} = value; } }"
      prefix = ''
    else
      l = "#{ws}// count = #{count}\n"
      l << "#{ws}public #{type}[] #{name}#{rest}"
    end

    if type == 'Byte'
      reader[-1] = "this.#{name} = r.ReadBytes(#{count});"
    elsif readfn.include? type
      reader[-1] = "this.#{name} = new #{type}[#{prefix}#{count}]; for (int i=0; i<#{prefix}#{count}; i++) this.#{name}[i] = #{readfn[type]};"
    else
      reader[-1] = "this.#{name} = new #{type}[#{prefix}#{count}]; for (int i=0; i<#{prefix}#{count}; i++) { #{type} obj = new #{type}(); obj.read(r); this.#{name}[i] = obj; }"
    end
  end

  puts l
end

puts "}"
