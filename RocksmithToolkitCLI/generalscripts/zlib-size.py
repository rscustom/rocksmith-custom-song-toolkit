import sys
import zlib

def zipstreams(filename):
    """Return all zip streams and their positions in file."""
    with open(filename, 'rb') as fh:
        data = fh.read()
    i = 0
    while i < len(data):
        try:
            zo = zlib.decompressobj()
            yield i, zo.decompress(data[i:])
            print 'zlib length:', len(data) - len(zo.unused_data) - i
            i += len(data[i:]) - len(zo.unused_data)
        except zlib.error:
            i += 1+3

fn = sys.argv[1]
print(fn)
for i, data in zipstreams(fn):
    print (i, len(data))
    #print data
