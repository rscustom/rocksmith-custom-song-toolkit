import json
import os
import struct
import sys

class BinReader(object):
    def __init__(self, f):
        self.fp = open(f, 'rb')
        self.size = os.path.getsize(f)

    def raw(self, n):
        return self.fp.read(n)

    def eof(self):
        return self.size == self.fp.tell()

    def long(self):
        return struct.unpack('<l', self.raw(4))[0]

    def double(self):
        return struct.unpack('<d', self.raw(8))[0]

    def short(self):
        return struct.unpack('<h', self.raw(2))[0]

    def float(self):
        return struct.unpack('<f', self.raw(4))[0]

    def byte(self):
        return struct.unpack('b', self.raw(1))[0]

    def cString(self, n):
        u = self.raw(n)
        i = 0
        while i < n:
            if ord(u[i]) == 0:
                break;
            i += 1
        return u[:i]

    def arr(self, getter):
        ret = []
        n = self.long()
        i = 0
        while i < n:
            ret.append(getter(self))
            i += 1
        return ret

    def arrN(self, getter, n):
        ret = []
        i = 0
        while i < n:
            ret.append(getter(self))
            i += 1
        return ret

class SngParser:
    @staticmethod
    def parse_beat(x):
        return {
            'time':         x.float(),
            'measure':      x.short(),
            'beat':         x.short(),
            'phrase_iter':  x.long(),
            'measure_mask': x.long()
        }
    
    @staticmethod
    def parse_phrase(x):
        return {
            'solo':            x.byte(),
            'disparity':       x.byte(),
            'ignore':          x.byte(),
            'padding':         x.byte(),
            'max_arrangement': x.long(),
            'iteration_count': x.long(),
            'name':            x.cString(32)
        }
    
    @staticmethod
    def parse_chord(x):
        return {
            'mask':    x.long(),
            'frets':   [x.byte() for k in range(6)],
            'fingers': [x.byte() for k in range(6)],
            'notes':   [x.long() for k in range(6)],
            'name':    x.cString(32)
        }
    
    @staticmethod
    def parse_bend_data(x):
        return {
            'time':      x.float(),
            'step':      x.float(),
            'unknown_0': x.short(),
            'unknown_1': x.byte(),
            'unknown_2': x.byte()
        }
    
    @staticmethod
    def parse_32_bend_data(x):
        bends = x.arrN(SngParser.parse_bend_data, 32)
        count = x.long()
        return bends[:count]
    
    @staticmethod
    def parse_chord_notes(x):
        return {
            'mask':       [x.long() for k in range(6)],
            'bend_data':  [SngParser.parse_32_bend_data(x) for k in range(6)],
            'start_fret': [x.byte() for k in range(6)],
            'end_fret':   [x.byte() for k in range(6)],
            'unknown':    [x.short() for k in range(6)]
        }
    
    @staticmethod
    def parse_vocal(x):
        return {
            'time':   x.float(),
            'note':   x.long(),
            'length': x.float(),
            'lyrics': x.cString(48)  # encoding to check 
        }
    
    @staticmethod
    def parse_symbols_header(x): # font related
        return {
            'unknown': [x.long() for k in range(8)]
        }
    
    @staticmethod
    def parse_symbols_texture(x):
        return {
            'fonts': x.cString(144)
        }
    
    @staticmethod
    def parse_symbols_definition(x):
        return {
            'text'    : x.cString(12).decode('utf8'),
            'y0x0'    : (x.float(), x.float()),
            'y1x1'    : (x.float(), x.float()),
            'y0x0_bb' : (x.float(), x.float()),
            'y1x1_bb' : (x.float(), x.float())
        }
    
    @staticmethod
    def parse_phrase_iter(x):
        return {
            'phrase':       x.long(),
            'start':        x.float(),
            'end':          x.float(),
            'arrangements': [x.long() for k in range(3)]  # seem to define the view box (in frets)
        }
    
    @staticmethod
    def parse_phrase_extra_info(x):
        return {
            'id':         x.long(),
            'difficulty': x.long(),
            'empty':      x.long(),
            'level_jump': x.byte(),
            'redundant':  x.short(),
            'padding':    x.byte()
        }
    
    @staticmethod
    def parse_linked_difficulty(x):
        return {
            'level_break' : x.long(),
            'NDL_phrases' : x.arr(BinReader.long)
        }
    
    @staticmethod
    def parse_action(x):
        return {
            'action': x.cString(256)
        }
    
    @staticmethod
    def parse_event(x):
        return {
            'time': x.float(),
            'name': x.cString(256)
        }
    
    @staticmethod
    def parse_tone(x):
        return {
            'time': x.float(),
            'tone': x.long()
        }
    
    @staticmethod
    def parse_dna(x):
        return {
            'time': x.float(),
            'dna' : x.long()
        }
    
    @staticmethod
    def parse_section(x):
        return {
            'name':       x.cString(32),
            'play_count': x.long(),
            'start':      x.float(),
            'end':        x.float(),
            'start_iter': x.long(),
            'end_iter':   x.long(),
            'unknown':    [x.byte() for k in range(36)] # what could it be ?
        }
    
    @staticmethod
    def parse_anchor(x):
        return {
            'start':       x.float(),
            'end':         x.float(),
            'unknown':     x.arrN(BinReader.float, 2),  # sometimes it's 'ffff7f7f00008000'
            'fret':        x.long(),
            'width':       x.long(),
            'phrase_iter': x.long()
        }
    
    @staticmethod
    def parse_anchor_extension(x):
        return {
            'beat':     x.long(),
            'fret':     x.byte(),
            'unknown':  x.raw(4 + 2 +1)
        }
    
    @staticmethod
    def parse_fingerprint(x):
        return {
            'chord':   x.long(),
            'start':   x.float(),
            'end':     x.float(),
            'unknown': x.arrN(BinReader.float, 2)
        }
    
    @staticmethod
    def parse_note(x):
	# comments are offset in the class GRNote
        return {
            'mask':         map(hex, x.arrN(BinReader.long, 2)), #0 4
            # BYPASS
            #'u0':           x.raw(4).encode('hex'), # 8
            'u0':           x.fp.read(4) and False,
            'time':         x.float(), #c
            'string':       x.byte(), #10
            'fret':         x.arrN(BinReader.byte, 2), #11 12
            'u1':           x.byte(), #13
            'chord':        x.long(), #14
            # BYPASS
            #'chord_notes':  x.long(), #18
            'chord_notes':  x.fp.read(4) and False,
            'phrase':       x.long(), #1c
            'phrase_iter':  x.long(), #20
            'fingerprints': x.arrN(BinReader.short, 2), #24 26
            'u2':           x.arrN(BinReader.short, 3), #2a 2c 2e
            'fingers':      x.arrN(BinReader.byte, 4), #34 35 32 33
            'pickDir':      x.byte(), #36
            'slap':         x.byte(), #37
            'pluck':        x.byte(), #38
            'vibrato':      x.short(), #3a
            'sustain':      x.float(), #3c
            'max_bend':     x.float(), #40 
            'bends':        x.arr(SngParser.parse_bend_data) #44    
        }
    
    @staticmethod
    def parse_arrangement(x):
        return {
            'difficulty':         x.long(),
            'anchors':            x.arr(SngParser.parse_anchor),
            'anchor_extensions':  x.arr(SngParser.parse_anchor_extension),
            'fingers':            x.arrN(lambda y: y.arr(SngParser.parse_fingerprint), 2),
            'notes':              x.arr(SngParser.parse_note),
            'av_note_per_phrase': x.arr(BinReader.float),
            'notes_per_iter':     x.arrN(lambda y: y.arr(BinReader.long), 2)
        }
    
    @staticmethod
    def parse_metadata(x):
        return {
            'max_score':           x.double(),
            'max_note_and_chords': x.double(),
            'u0':                  x.double(),  # look like a reference frequency (in Hz)...
            'points_per_note':     x.double(),
            'first_beat_length':   x.float(),
            'start':               x.float(),
            'capo':                x.byte(),
            'conversion_date':     x.cString(32),
            'part':                x.short(),
            'song_length':         x.float(),
            'tuning':              x.arr(BinReader.short),
            'u1':                  x.arrN(BinReader.float, 2), # close to start time is a note time
            'max_difficulty':      x.long()
        }
    
def parseSNG(y):
    x = BinReader(y)
    r = {}
    r['beats'] =       x.arr(SngParser.parse_beat)
    r['phrases'] =     x.arr(SngParser.parse_phrase)
    r['chords'] =      x.arr(SngParser.parse_chord)
    r['chord_notes'] = x.arr(SngParser.parse_chord_notes)

    r['vocals'] = x.arr(SngParser.parse_vocal)
    if len(r['vocals']) > 0:
        # fonts
        r['symbols'] = {
            'headers':     x.arr(SngParser.parse_symbols_header),
            'textures':    x.arr(SngParser.parse_symbols_texture),
            'definitions': x.arr(SngParser.parse_symbols_definition)
        }

    r['phrase_iters']      = x.arr(SngParser.parse_phrase_iter)
    r['phrase_extra_info'] = x.arr(SngParser.parse_phrase_extra_info)
    r['linked_difficulty'] = x.arr(SngParser.parse_linked_difficulty)
    r['actions']           = x.arr(SngParser.parse_action)
    r['events']            = x.arr(SngParser.parse_event)
    r['tones']             = x.arr(SngParser.parse_tone)
    r['dna']               = x.arr(SngParser.parse_dna)
    r['sections']          = x.arr(SngParser.parse_section)
    r['arrangements']      = x.arr(SngParser.parse_arrangement)
    r['metadata']          = SngParser.parse_metadata(x)

    assert x.eof()
    return r

import sys
from datadiff import diff
a = parseSNG(sys.argv[1])
b = parseSNG(sys.argv[2])

# some field are bypassed, this code messes diff up for some reason
# for sng in (a,b):
#     for a in sng['arrangements']:
#         for n in a['notes']:
#             # we know this Hash is different and it should be ok
#             n['u0'] = None
#             # we know this is different too and it should be ok too
#             if n['chord_notes'] != -1:
#                 n['chord_notes'] = None

print diff(a,b)
