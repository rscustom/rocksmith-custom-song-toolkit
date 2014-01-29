#!/usr/bin/env python

import struct
import os
import json
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


def parse_BPM(x):
    return {
        'time':        x.float(),
        'measure':     x.short(),
        'beat':        x.short(),
        'phraseIter':  x.long(),
        'measureMask': x.long()
    }

def parse_phrase(x):
    return {
        'solo':                   x.byte(),
        'disparity':              x.byte(),
        'ignore':                 x.byte(),
        'padding':                x.byte(),
        'max_difficulty':         x.long(),
        'phrase_iteration_links': x.long(),
        'name':                   x.cString(32)
    }

def parse_chord(x):
    return {
        'mask':    x.long(),
        'frets':   [x.byte() for k in range(6)],
        'fingers': [x.byte() for k in range(6)],
        'notes':   [x.long() for k in range(6)],
        'name':    x.cString(32)
    }

def parse_bend_data(x):
    return {
        'time':      x.float(),
        'step':      x.float(),
        'unknown_0': x.short(),
        'unknown_1': x.byte(),
        'unknown_2': x.byte()
    }

def parse_32_bend_data(x):
    bends = x.arrN(parse_bend_data, 32)
    count = x.long()
    return bends[:count]

def parse_chord_notes(x):
    return {
        'notemask':    [x.long() for k in range(6)],
        'benddata':    [parse_32_bend_data(x) for k in range(6)],
        'startFretId': [x.byte() for k in range(6)],
        'endFretId':   [x.byte() for k in range(6)],
        'unknown':     [x.short() for k in range(6)]
    }

def parse_vocal(x):
    return {
        'time':   x.float(),
        'note':   x.long(),
        'length': x.float(),
        'lyrics': x.cString(48)
    }

def parse_symbols_header(x):
    return {
        'unknown': [x.long() for k in range(8)]
    }

def parse_symbols_texture(x):
    return {
        'dds': x.cString(144)
    }

def parse_symbols_definition(x):
    return {
        'text': x.cString(12).decode('utf8'),
        'y0'   : x.float(),
        'x0'   : x.float(),
        'y1'   : x.float(),
        'x1'   : x.float(),
        'bb_y0': x.float(),
        'bb_x0': x.float(),
        'bb_y1': x.float(),
        'bb_x1': x.float(),
    }

def parse_phrase_iter(x):
    return {
        'id':         x.long(),
        'start':      x.float(),
        'nextPhrase': x.float(),
        'unknown':    [x.long() for k in range(3)]  # seem to define the view box (in frets)
    }

def parse_phrase_extra_info(x):
    return {
        'id':         x.long(),
        'difficulty': x.long(),
        'empty':      x.long(),
        'level_jump': x.byte(),
        'redundant':  x.short(),
        'padding':    x.byte()
    }

def parse_linked_difficulty(x):
    return {
        'level_break': x.long(),
        'NDL_phrase':  x.arr(BinReader.long)
    }

def parse_action(x):
    return {
        'action': x.cString(256)
    }

def parse_event(x):
    return {
        'time':  x.float(),
        'name': x.cString(256)
    }

def parse_tone(x):
    return {
        'time': x.float(),
        'tone': x.long()
    }

def parse_dna(x):
    return {
        'time': x.float(),
        'dna':  x.long()
    }

def parse_section(x):
    return {
        'name':                  x.cString(32),
        'play_count':            x.long(),
        'start':                 x.float(),
        'end':                   x.float(),
        'start_phraseIteration': x.long(),
        'end_phraseIteration':   x.long(),
        'unknown':               [x.byte() for k in range(36)] # what could it be ?
    }

def parse_anchor(x):
    return {
        'startBeat':  x.float(),
        'endBeat':    x.float(),
        'unknown':    x.arrN(BinReader.float, 2),  # sometimes it's 'ffff7f7f00008000'
        'fret':       x.long(),
        'width':      x.long(),
        'phraseIter': x.long()
    }

def parse_anchor_extension(x):
    return {
        'beatTime': x.long(),
        'fret':     x.byte(),
        'unknown':  x.raw(4 + 2 +1)
    }

def parse_fingerprint(x):
    return {
        'chord':   x.long(),
        'start':   x.float(),
        'end':     x.float(),
        'unknown': x.arrN(BinReader.float, 2)
    }

def parse_note(x):
    return {
        'mask':         map(hex, x.arrN(BinReader.long, 2)),
        'u0':           x.raw(4).encode('hex'),
        'time':         x.float(),
        'string':       x.byte(),
        'fret':         x.arrN(BinReader.byte, 2),
        'u1':           x.byte(),
        'chord':        x.long(),
        'chordNotes':   x.long(),
        'phrase':       x.long(),
        'phraseIter':   x.long(),
        'fingerprints': x.arrN(BinReader.short, 2),
        'u2':           x.arrN(BinReader.short, 3),
        'fingers':      x.arrN(BinReader.byte, 4),
        'pickDir':      x.byte(),
        'slap':         x.byte(),
        'pluck':        x.byte(),
        'vibrato':      x.short(),
        'sustain':      x.float(),
        'maxbend':      x.float(),
        'bends':        x.arr(parse_bend_data)
    }

def parse_arrangement(x):
    return {
        'difficulty':            x.long(),
        'anchors':               x.arr(parse_anchor),
        'anchor_ext':            x.arr(parse_anchor_extension),
        'fingers':               x.arrN(lambda y: y.arr(parse_fingerprint), 2),
        'notes':                 x.arr(parse_note),
        'average_note_per_iter': x.arr(BinReader.float),
        'notes_in_iter':         x.arrN(lambda y: y.arr(BinReader.long), 2)
    }

def parse_metadata(x):
    return {
        'maxscore':            x.double(),
        'max_note_and_chords': x.double(),
        'u0':                  x.double(),  # look like a reference frequency (in Hz)...
        'points_per_note':     x.double(),
        'first_beat_length':   x.float(),
        'startTime':           x.float(),
        'capoFret':            x.byte(),
        'lastConversionDate':  x.cString(32),
        'part':                x.short(),
        'song_length':         x.float(),
        'tuning':              x.arr(BinReader.short),
        'u1':                  x.arrN(BinReader.float, 2), # start time intro section * 2
        'maxdifficulty':       x.long()
    }

def parse(x):
    r = {}
    r['bpms'] =        x.arr(parse_BPM)
    r['phrases'] =     x.arr(parse_phrase)
    r['chords'] =      x.arr(parse_chord)
    r['chord_notes'] = x.arr(parse_chord_notes)

    r['vocals'] = x.arr(parse_vocal)
    if len(r['vocals']) > 0:
	# font description
        r['symbols'] = {
            'headers':     x.arr(parse_symbols_header),
            'textures':    x.arr(parse_symbols_texture),
            'definitions': x.arr(parse_symbols_definition)
        }

    r['phrases_iter']      = x.arr(parse_phrase_iter)
    r['phrase_extra_info'] = x.arr(parse_phrase_extra_info)
    r['linked_difficulty'] = x.arr(parse_linked_difficulty)
    r['actions']           = x.arr(parse_action)
    r['events']            = x.arr(parse_event)
    r['tones']             = x.arr(parse_tone)
    r['dna']               = x.arr(parse_dna)
    r['sections']          = x.arr(parse_section)
    r['arrangements']      = x.arr(parse_arrangement)
    r['metadata']          = parse_metadata(x)

    assert x.eof()
    return r

if __name__ == "__main__":
    f = BinReader( sys.argv[1] )
    y = parse(f)
    print json.dumps(y)