import zlib
import struct
import os
import sys

f = open(sys.argv[1], 'rb')
raw = f.read()
f.close()

#numfiles = struct.unpack('>L', raw[20:24])[0]

path = 'output/'
d = os.path.dirname(path)
if not os.path.exists(path):
    os.makedirs(path)

import re

for o in [m.start() for m in re.finditer("RIFF", raw)]:
    unpacked = raw[o:]
    print 'FOUND', o
    p = path + str(o)
    g = open (p, 'wb')
    g.write(unpacked)
    g.close()
