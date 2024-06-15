extends Node
class_name MusicPlayer

class Sample:
    var clip: AudioStream
    var start_note: String

    func _init(clip: AudioStream, start_note: String):
        self.clip = clip
        self.start_note = start_note

const INSTRUMENT_PATH = "res://Main/Art/Audio/"

@export_multiline var song: String = """
X:1
Q:1/4=178
L:1/4
M:4/4
K:C clef=G2
V:1
%%Music/Instruments/Piano
z16|z16|z16|z16|
z16|z16|a4a4a2e2a4|a4^d2^d2a4a4|
^d4b4b4b2^d2|a4a4a2e2a4|a4^d2^d2a4a4|^d4b4b4b2^d2|
^c4^c8^g4|^g4^c2e2a4a4|^d2^d2b4^g4b2^d2|a4a4a2e2a4|
a4^d2^d2a4a4|^d4b4b4b2^d2|a4e6a2z4|z4b2e2z4^d4|
a2a2b4^g6e2|^d4a4^d2^d2^f4|^c4^f2a2b4^g4|b2e2^g4e4^g2^c2|
a4e6a2z4|z4b2e2z4^d4|a2a2b4^g6e2|^d4a4^d2^d2^f4|
^c4^f2a2b4^g4|b2e2^g4e4^g2^c2|a4a4a2e2a4|a4^d2^d2a4a4|
^d4b4b4b2^d2|a4a4a2e2a4|a4^d2^d2a4a4|^d4b4b4b2^d2|
e4e4^c2a2z4|^d4a2^f2a4a4-|a2^d2^d4^f4^d2b2|e4e4^c2a2z4|
^d4a2^f2a4a4-|a2^d2^d4^f4^d2b2|a4a4a2e2a4|a4^d2^d2a4a4|
^d4b4b4b2^d2|a4a4a2e2a4|a4^d2^d2a4a4|^d4b4b4b2^d2|
^c4^c8^g4|^g4^c2e2a4a4|^d2^d2b4^g4b2^d2|a4a4a2e2a4|
a4^d2^d2a4a4|^d4b4b4b2^d2|a4e6a2z4|z4b2e2z4^d4|
a2a2b4^g6e2|a4e6a2z4|z4b2e2z4^d4|a2a2b4^g6e2|
^c4e4e4b4|e4e2b2^f4a4|a2^f2b4e4e2b2|a4e6a2z4|
z4b2e2z4^d4|a2a2b4^g6e2|a4e6a2z4|z4b2e2z4^d4|
a2a2b4^g6e2|^d4a4^d2^d2^f4|^c4^f2a2b4^g4|b2e2^g4e4^g2^c2|
a4e6a2z4|z4b2e2z4^d4|a2a2b4^g6e2|^d4a4^d2^d2^f4|
^c4^f2a2b4^g4|b2e2^g4e4^g2^c2|z2^c2^c6a2z2^g2|^g6e2z2^d2^d2^g2|
^g2^g2z2^g2^g2b2b2^d2|z2^c2^c6a2z2^g2|^g6e2z2^d2^d2^g2|^g2^g2z2^g2^g2b2b2^d2|
z2^c2^c2e2e2^g2z2a2-|a2^c6z2^c2^c2e2-|e2a2z4^g2b4^d2|z2^c2^c6a2z2^g2|
^g6e2z2^d2^d2^g2|^g2^g2z2^g2^g2b2b2^d2|a4e6a2z4|z4b2e2z4^d4|
a2a2b4^g6e2|^d4a4^d2^d2^f4|^c4^f2a2b4^g4|b2e2^g4e4^g2^c2|
a4e6a2z4|z4b2e2z4^d4|a2a2b4^g6e2|^d4a4^d2^d2^f4|
^c4^f2a2b4^g4|b2e2^g4e4^g2^c2|a4e6a2z4|z4b2e2z4^d4|
a2a2b4^g6e2|a4e6a2z4|z4b2e2z4^d4|a2a2b4^g6e2|
^f4^c6a2^d4|b4^d2^f2b4^f4|b2b2^d4b4^d2^f2|^f4^c6a2^d4|
b4^d2^f2b4^f4|b2b2^d4b4^d2^f2|^C16-|^C16|
V:2
%%Music/Instruments/Piano
A,,2A,,8A,,2^D,,2^D,,2-|^D,,2^D,,4^D,,2^D,,8-|^D,,4B,,2B,,8B,,2|A,,2A,,8A,,2^D,,2^D,,2-|
^D,,2^D,,4^D,,2^D,,8-|^D,,4B,,2B,,8B,,2|A,,2A,,8A,,2^D,,2^D,,2-|^D,,2^D,,4^D,,2^D,,8-|
^D,,4B,,2B,,8B,,2|A,,2A,,8A,,2^D,,2^D,,2-|^D,,2^D,,4^D,,2^D,,8-|^D,,4B,,2B,,8B,,2|
^C,,2^C,,8^C,,2^C,,2^C,,2-|^C,,8^D,,2^D,,6-|^D,,2^D,,2^G,,2^G,,8^G,,2|A,,2A,,8A,,2^D,,2^D,,2-|
^D,,2^D,,4^D,,2^D,,8-|^D,,4B,,2B,,8B,,2|A,,2A,,8A,,2E,,2E,,2-|E,,6E,,2^D,,2^D,,6-|
^D,,2^D,,2E,,2E,,8E,,2|^D,,2^D,,4^D,,4^D,,2^F,,2^F,,2-|^F,,6^F,,2E,,2E,,6-|E,,2E,,2^C,,2^C,,8^C,,2|
A,,2A,,8A,,2E,,2E,,2-|E,,6E,,2^D,,2^D,,6-|^D,,2^D,,2E,,2E,,8E,,2|^D,,2^D,,4^D,,4^D,,2^F,,2^F,,2-|
^F,,6^F,,2E,,2E,,6-|E,,2E,,2^C,,2^C,,8^C,,2|A,,2A,,6A,,2A,,2^D,,2^D,,2-|^D,,6^D,,2^D,,2^D,,6-|
^D,,2^D,,2B,,2B,,8B,,2|A,,2A,,6A,,2A,,2^D,,2^D,,2-|^D,,6^D,,2^D,,2^D,,6-|^D,,2^D,,2B,,2B,,8B,,2|
A,,2A,,2A,,2A,,4A,,2^D,,2^D,,2|^D,,2^D,,2^D,,2^D,,2^D,,2^D,,2^D,,4|^D,,2^D,,2B,,2B,,2B,,2B,,2B,,2B,,2|A,,2A,,2A,,2A,,4A,,2^D,,2^D,,2|
^D,,2^D,,2^D,,2^D,,2^D,,2^D,,2^D,,4|^D,,2^D,,2B,,2B,,2B,,2B,,2B,,2B,,2|A,,2A,,8A,,2^D,,2^D,,2-|^D,,2^D,,4^D,,2^D,,8-|
^D,,4B,,2B,,8B,,2|A,,2A,,8A,,2^D,,2^D,,2-|^D,,2^D,,4^D,,2^D,,8-|^D,,4B,,2B,,8B,,2|
^C,,2^C,,8^C,,2^C,,2^C,,2-|^C,,8^D,,2^D,,6-|^D,,2^D,,2^G,,2^G,,8^G,,2|A,,2A,,8A,,2^D,,2^D,,2-|
^D,,2^D,,4^D,,2^D,,8-|^D,,4B,,2B,,8B,,2|A,,8-A,,2A,,2E,,2E,,2-|E,,8^D,,2^D,,6-|
^D,,2^D,,2E,,2E,,8E,,2|A,,8-A,,2A,,2E,,2E,,2-|E,,8^D,,2^D,,6-|^D,,2^D,,2E,,2E,,8E,,2|
A,,2A,,2A,,4A,,2A,,2E,,2E,,2|E,,4E,,2E,,2^D,,2^D,,2^D,,4-|^D,,2^D,,2E,,2E,,2E,,4E,,2E,,2|A,,8-A,,2A,,2E,,2E,,2-|
E,,8^D,,2^D,,6-|^D,,2^D,,2E,,2E,,8E,,2|A,,2A,,8A,,2E,,2E,,2-|E,,6E,,2^D,,2^D,,6-|
^D,,2^D,,2E,,2E,,8E,,2|^D,,2^D,,4^D,,4^D,,2^F,,2^F,,2-|^F,,6^F,,2E,,2E,,6-|E,,2E,,2^C,,2^C,,8^C,,2|
A,,2A,,8A,,2E,,2E,,2-|E,,6E,,2^D,,2^D,,6-|^D,,2^D,,2E,,2E,,8E,,2|^D,,2^D,,4^D,,4^D,,2^F,,2^F,,2-|
^F,,6^F,,2E,,2E,,6-|E,,2E,,2^C,,2^C,,8^C,,2|^F,,2^F,,8^F,,2^C,,2^C,,2-|^C,,6^C,,2^G,,2^G,,6-|
^G,,2^G,,2^G,,2^G,,8^G,,2|^F,,2^F,,8^F,,2^C,,2^C,,2-|^C,,6^C,,2^G,,2^G,,6-|^G,,2^G,,2^G,,2^G,,8^G,,2|
^C,,2^C,,6^C,,2^C,,2^F,,2^F,,2-|^F,,4^F,,2^F,,2z2A,,6-|A,,4^G,,2^G,,8^G,,2|^F,,2^F,,8^F,,2^C,,2^C,,2-|
^C,,6^C,,2^G,,2^G,,6-|^G,,2^G,,2^G,,2^G,,8^G,,2|A,,2A,,8A,,2E,,2E,,2-|E,,6E,,2^D,,2^D,,6-|
^D,,2^D,,2E,,2E,,8E,,2|^D,,2^D,,4^D,,4^D,,2^F,,2^F,,2-|^F,,6^F,,2E,,2E,,6-|E,,2E,,2^C,,2^C,,8^C,,2|
A,,2A,,8A,,2E,,2E,,2-|E,,6E,,2^D,,2^D,,6-|^D,,2^D,,2E,,2E,,8E,,2|^D,,2^D,,4^D,,4^D,,2^F,,2^F,,2-|
^F,,6^F,,2E,,2E,,6-|E,,2E,,2^C,,2^C,,8^C,,2|A,,2A,,8A,,2E,,2E,,2-|E,,6E,,2^D,,2^D,,6-|
^D,,2^D,,2E,,2E,,8E,,2|A,,2A,,8A,,2E,,2E,,2-|E,,6E,,2^D,,2^D,,6-|^D,,2^D,,2E,,2E,,8E,,2|
^F,,2^F,,8^F,,2B,,2B,,2-|B,,6B,,2B,,2B,,6-|B,,2B,,2B,,2B,,8B,,2|^F,,2^F,,8^F,,2B,,2B,,2-|
B,,6B,,2B,,2B,,6-|B,,2B,,2B,,2B,,8B,,2|^C,,16-|^C,,16|
V:3
%%Music/Instruments/Piano
z2E,2E,2E,2A,,2A,,2z2A,2|A,2A,2^D,2^D,2z2^D2^D2^D2|^F,2^F,2z2B,2B,2B,2^D,2^D,2|z2E,2E,2E,2A,,2A,,2z2A,2|
A,2A,2^D,2^D,2z2^D2^D2^D2|^F,2^F,2z2B,2B,2B,2^D,2^D,2|z2E,2E,2E,2A,,2A,,2z2A,2|A,2A,2^D,2^D,2z2^D2^D2^D2|
^F,2^F,2z2B,2B,2B,2^D,2^D,2|z2E,2E,2E,2A,,2A,,2z2A,2|A,2A,2^D,2^D,2z2^D2^D2^D2|^F,2^F,2z2B,2B,2B,2^D,2^D,2|
z2^C2^C2^C2E,2E,2z2^G,2|^G,2^G,2^C,2^C,2z2^D2^D2^D2|^F,2^F,2z2B,,2B,,2B,,2^D,,2^D,,2|z2E,2E,2E,2A,,2A,,2z2A,2|
A,2A,2^D,2^D,2z2^D2^D2^D2|^F,2^F,2z2B,2B,2B,2^D,2^D,2|z4[A,^CE,]8z4|[E^G,B,]8z4[^D,-^F,-A,,-]4|
[^D,^F,A,,]4z4[E^G,B,]8|z4[^D^F,A,]8z4|[^FA,^C]8z4[E,-^G,-B,-]4|[E,^G,B,]4z4[^C,E,^G,,]8|
z4[A,^CE,]8z4|[E^G,B,]8z4[^D,-^F,-A,,-]4|[^D,^F,A,,]4z4[E^G,B,]8|z4[^D^F,A,]8z4|
[^FA,^C]8z4[E,-^G,-B,-]4|[E,^G,B,]4z4[^C,E,^G,,]8|z2[A^CE]2[A,^CE,]2[A,^CE,]2[E,,E,]2[E,,E,]2z2[^D^F,A,]2|[^D^F,A,]2[^D,^F,A,]2[^D,,^D,]2[^D,,^D,]2z2[^D,^F,A,,]2[^D,^F,A,]2[^D,^F,A,,]2|
[^F,,^F,]2[^F,,^F,]2z2[B^D^F]2[B,^D^F]2[B^D^F]2[B,,B,]2[B,,B,]2|z2[A^CE]2[A,^CE,]2[A,^CE,]2[E,,E,]2[E,,E,]2z2[^D^F,A,]2|[^D^F,A,]2[^D,^F,A,]2[^D,,^D,]2[^D,,^D,]2z2[^D,^F,A,,]2[^D,^F,A,]2[^D,^F,A,,]2|[^F,,^F,]2[^F,,^F,]2z2[B^D^F]2[B,^D^F]2[B^D^F]2[B,,B,]2[B,,B,]2|
z2^C,2^C,2^C,2E,,2E,,2z2A,2|A,2A,2^D,2^D,2z2^F,2^F,2^F,2|A,,2A,,2z2^D,2^D,2^D,2^F,,2^F,,2|z2^C,2^C,2^C,2E,,2E,,2z2A,2|
A,2A,2^D,2^D,2z2^F,2^F,2^F,2|A,,2A,,2z2^D,2^D,2^D,2^F,,2^F,,2|z2E,2E,2E,2A,,2A,,2z2A,2|A,2A,2^D,2^D,2z2^D2^D2^D2|
^F,2^F,2z2B,2B,2B,2^D,2^D,2|z2E,2E,2E,2A,,2A,,2z2A,2|A,2A,2^D,2^D,2z2^D2^D2^D2|^F,2^F,2z2B,2B,2B,2^D,2^D,2|
z2^C2^C2^C2E,2E,2z2^G,2|^G,2^G,2^C,2^C,2z2^D2^D2^D2|^F,2^F,2z2B,,2B,,2B,,2^D,,2^D,,2|z2E,2E,2E,2A,,2A,,2z2A,2|
A,2A,2^D,2^D,2z2^D2^D2^D2|^F,2^F,2z2B,2B,2B,2^D,2^D,2|z4E,8z4|^G,8z4A,4-|
A,4z4^G,8|z4E,8z4|^G,8z4A,4-|A,4z4^G,8|
z4[A,^CE,]8z4|[E,^G,B,]8z4[^D,-^F,-A,-]4|[^D,^F,A,]4z4[E,^G,B,]8|z4E,8z4|
^G,8z4A,4-|A,4z4^G,8|z4[A,^CE,]8z4|[E^G,B,]8z4[^D,-^F,-A,,-]4|
[^D,^F,A,,]4z4[E^G,B,]8|z4[^D^F,A,]8z4|[^FA,^C]8z4[E,-^G,-B,-]4|[E,^G,B,]4z4[^C,E,^G,,]8|
z4[A,^CE,]8z4|[E^G,B,]8z4[^D,-^F,-A,,-]4|[^D,^F,A,,]4z4[E^G,B,]8|z4[^D^F,A,]8z4|
[^FA,^C]8z4[E,-^G,-B,-]4|[E,^G,B,]4z4[^C,E,^G,,]8|[^F,A,^C,]12[^C,-E,-^G,,-]4|[^C,E,^G,,]8[^G-B,-^D-]8|
[^GB,^D]4[^GB,^D]12|[^F,A,^C,]12[^C,-E,-^G,,-]4|[^C,E,^G,,]8[^G-B,-^D-]8|[^GB,^D]4[^GB,^D]12|
[^C,E,^G,]12[^F-A,-^C-]4|[^FA,^C]8[A,-^C-E-]8|[A,^CE]4[^G,B,^D]12|[^F,A,^C,]12[^C,-E,-^G,,-]4|
[^C,E,^G,,]8[^G-B,-^D-]8|[^GB,^D]4[^GB,^D]12|z4[A,^CE,]8z4|[E^G,B,]8z4[^D,-^F,-A,,-]4|
[^D,^F,A,,]4z4[E^G,B,]8|z4[^D^F,A,]8z4|[^FA,^C]8z4[E,-^G,-B,-]4|[E,^G,B,]4z4[^C,E,^G,,]8|
z4[A,^CE,]8z4|[E^G,B,]8z4[^D,-^F,-A,,-]4|[^D,^F,A,,]4z4[E^G,B,]8|z4[^D^F,A,]8z4|
[^FA,^C]8z4[E,-^G,-B,-]4|[E,^G,B,]4z4[^C,E,^G,,]8|z4[A,^CE,]8z4|[E^G,B,]8z4[^D,-^F,-A,,-]4|
[^D,^F,A,,]4z4[E^G,B,]8|z4[A,^CE,]8z4|[E^G,B,]8z4[^D,-^F,-A,,-]4|[^D,^F,A,,]4z4[E^G,B,]8|
z4[^F,A,^C,]8z4|[B,^D^F,]8z4[B,-^D-^F-]4|[B,^D^F]4z4[B^D^F]8|z4[^F,A,^C,]8z4|
[B,^D^F,]8z4[B,-^D-^F-]4|[B,^D^F]4z4[B^D^F]8|[^C,-E,-^G,-]16|[^C,E,^G,]16|
V:4
%%Music/Instruments/Percussion
z16|z16|z16|D,,2z2D,,2D,,2D,,2z4D,,2|
z4D,,2D,,2D,,2z4D,,2|D,,2D,,2D,,2D,,2D,,2D,,2D,,2D,,2|C,,2C,,2z2[C,,D,,]2z4C,,2z2|[C,,D,,]2z2C,,2D,,2C,,2C,,2z2[C,,D,,]2|
z4C,,2z2[C,,D,,]2z2C,,2D,,2|C,,2C,,2z2[C,,D,,]2z4C,,2z2|[C,,D,,]2z2C,,2D,,2C,,2C,,2z2[C,,D,,]2|z4C,,2z2[C,,D,,]2z2C,,2D,,2|
C,,2C,,2z2[C,,D,,]2z4C,,2z2|[C,,D,,]2z2C,,2D,,2C,,2C,,2z2[C,,D,,]2|z4C,,2z2[C,,D,,]2z2C,,2D,,2|C,,2[C,,D,,]2z2C,,2D,,2z2C,,2z2|
C,,2z2C,,2D,,2[C,,D,,]2[C,,D,,]2D,,2C,,2|z4[C,,D,,]2z2[C,,D,,]2D,,2[C,,D,,]2D,,2|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|
[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|
[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|
[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|[C,,-^G,,][C,,]z2[C,,-D,,-^G,,][C,,D,,]^G,,zC,,2^G,,zC,,2^G,,z|C,,2C,,2C,,2^G,,z[C,,-^G,,][C,,]z2[C,,-D,,-^G,,][C,,D,,]^G,,z|
C,,2^G,,zC,,2^G,,zC,,2C,,2C,,2^G,,z|[C,,-^G,,][C,,]z2[C,,-D,,-^G,,][C,,D,,]^G,,zC,,2^G,,zC,,2^G,,z|C,,2C,,2C,,2^G,,z[C,,-^G,,][C,,]z2[C,,-D,,-^G,,][C,,D,,]^G,,z|C,,2^G,,zC,,2^G,,zC,,2C,,2C,,2^G,,z|
[C,,-^G,,][C,,]z2[C,,-D,,-^G,,][C,,D,,]^G,,zC,,2^G,,zC,,2^G,,z|C,,2C,,2C,,2^G,,z[C,,-^G,,][C,,]z2[C,,-D,,-^G,,][C,,D,,]^G,,z|C,,2^G,,zC,,2^G,,zC,,2C,,2C,,2^G,,z|[C,,-^G,,][C,,]z2[C,,-D,,-^G,,][C,,D,,]^G,,zC,,2^G,,z[C,,D,,]2[D,,-^G,,][D,,]|
C,,2C,,2C,,2^G,,z[C,,-^G,,][C,,]D,,2[C,,-D,,-^G,,][C,,D,,]^G,,z|C,,2^G,,z[C,,D,,]2[D,,-^G,,][D,,][C,,D,,]2[C,,D,,]2[C,,D,,]2[D,,-^G,,][D,,]|C,,2C,,2z2[C,,D,,]2z4C,,2z2|[C,,D,,]2z2C,,2D,,2C,,2C,,2z2[C,,D,,]2|
z4C,,2z2[C,,D,,]2z2C,,2D,,2|C,,2C,,2z2[C,,D,,]2z4C,,2z2|[C,,D,,]2z2C,,2D,,2C,,2C,,2z2[C,,D,,]2|z4C,,2z2[C,,D,,]2z2C,,2D,,2|
C,,2C,,2z2[C,,D,,]2z4C,,2z2|[C,,D,,]2z2C,,2D,,2C,,2C,,2z2[C,,D,,]2|z4C,,2z2[C,,D,,]2z2C,,2D,,2|C,,2[C,,D,,]2z2C,,2D,,2z2C,,2z2|
C,,2z2C,,2D,,2[C,,D,,]2[C,,D,,]2D,,2C,,2|z4[C,,D,,]2z2[C,,D,,]2D,,2[C,,D,,]2D,,2|C,,2z2[C,,-^G,,][C,,][C,,-^G,,][C,,][C,,-D,,-^G,,][C,,D,,][C,,-^G,,][C,,]C,,2^G,,z|C,,2[D,,-^G,,][D,,][C,,D,,]2z2C,,2z2[C,,-^G,,][C,,][C,,-^G,,][C,,]|
[C,,-D,,-^G,,][C,,D,,][C,,-^G,,][C,,]C,,2^G,,zC,,2[D,,-^G,,][D,,][C,,D,,]2z2|C,,2z2[C,,-^G,,][C,,][C,,-^G,,][C,,][C,,-D,,-^G,,][C,,D,,][C,,-^G,,][C,,]C,,2^G,,z|C,,2[D,,-^G,,][D,,][C,,D,,]2z2C,,2z2[C,,-^G,,][C,,][C,,-^G,,][C,,]|[C,,-D,,-^G,,][C,,D,,][C,,-^G,,][C,,]C,,2^G,,zC,,2[D,,-^G,,][D,,][C,,D,,]2z2|
C,,2z2[C,,-^G,,][C,,][C,,-^G,,][C,,][C,,-D,,-^G,,][C,,D,,][C,,-^G,,][C,,]C,,2^G,,z|C,,2[D,,-^G,,][D,,][C,,D,,]2z2C,,2z2[C,,-^G,,][C,,][C,,-^G,,][C,,]|[C,,-D,,-^G,,][C,,D,,][C,,-^G,,][C,,]C,,2^G,,zC,,2[D,,-^G,,][D,,][C,,D,,]2z2|C,,2z2[C,,-^G,,][C,,][C,,-^G,,][C,,][C,,-^G,,][C,,][C,,-D,,-^G,,][C,,D,,]C,,2[D,,-^G,,][D,,]|
[C,,D,,]2^G,,zC,,2z2[C,,D,,]2z2[C,,-D,,-^G,,][C,,D,,][C,,-D,,-^G,,][C,,D,,]|[C,,-D,,-^G,,][C,,D,,][C,,-^G,,][C,,][C,,D,,]2[D,,-^G,,][D,,][C,,D,,]2[D,,-^G,,][D,,][C,,D,,]2D,,2|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|
[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|
[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|
[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|C,,2C,,2C,,2[C,,D,,]2[C,,D,,]2D,,2C,,2D,,2|z2D,,2C,,2C,,2C,,2C,,2C,,2[C,,D,,]2|
[C,,D,,]2D,,2C,,2D,,2z2D,,2C,,2C,,2|C,,2C,,2C,,2[C,,D,,]2[C,,D,,]2D,,2C,,2D,,2|z2D,,2C,,2C,,2C,,2C,,2C,,2[C,,D,,]2|[C,,D,,]2D,,2C,,2D,,2z2D,,2C,,2C,,2|
C,,2C,,2C,,2[C,,D,,]2[C,,D,,]2D,,2C,,2D,,2|z2D,,2C,,2C,,2C,,2C,,2C,,2[C,,D,,]2|[C,,D,,]2D,,2C,,2D,,2z2D,,2C,,2C,,2|C,,2[C,,D,,]2C,,2C,,2C,,2z2C,,2z2|
z2D,,2C,,2C,,2C,,2[C,,D,,]2[C,,D,,]2[C,,D,,]2|[C,,D,,]2D,,2[C,,D,,]2D,,2D,,2D,,2[C,,D,,]2[C,,D,,]2|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|
[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|
[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|
[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|
[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|
[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,][C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,]|[D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z[C,,-^G,,][C,,][C,,D,,]2[C,,-^G,,][C,,][D,,-^G,,][D,,]|[C,,D,,]2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-^G,,][C,,][D,,-^G,,][D,,][C,,-^G,,][C,,]^G,,z^G,,z|[C,,-^G,,][C,,]C,,2[C,,-^G,,][C,,][D,,-^G,,][D,,]C,,2z2[C,,-^G,,^C,][C,,][C,,-^G,,][C,,]|
^G,,z[C,,-^G,,][C,,]^G,,z[D,,-^G,,][D,,][C,,-D,,-^G,,][C,,D,,]C,,2[C,,-D,,-^G,,][C,,D,,][D,,-^G,,][D,,]|C,,2z2[C,,-D,,-^G,,^C,][C,,D,,][C,,-D,,-^G,,][C,,D,,]^G,,z[C,,-D,,-^G,,][C,,D,,][D,,-^G,,][D,,][D,,-^G,,][D,,]|[C,,-D,,-^G,,^C,][C,,D,,]
"""

@export var play_on_ready: bool = true

@export var debug: bool = false:
    set(value):
        debug = value
        ABCParser.debug = value

var instruments: Dictionary = {} # Dictionary to hold instrument samples for each track
var instruments_per_track: Dictionary = {} # Dictionary to hold instrument assignments per track
var parsed_notes = {}
var beats_per_minute: float = 120.0 # Default BPM
var loop_start_beat: float = 0.0 # Default loop start beat
var loop: bool = true # Enable looping by default
var is_playing: bool = false # Flag to control playback
var key_signature: String = "C"

func _ready():
    if play_on_ready:
        play()

func play(song_string: String = "", loop: bool = true):
    if song_string == "":
        song_string = song
    if debug:
        print("Playing song:\n", song_string)  # Debugging print
    parse_song_header(song_string)
    parsed_notes = ABCParser.parse_abc(song_string)
    cache_samples()
    apply_key_signature()
    var max_duration = calculate_max_duration(parsed_notes)
    var smallest_duration = get_smallest_duration(parsed_notes)
    play_notes(parsed_notes, max_duration, smallest_duration, loop)

func stop():
    is_playing = false

func parse_song_header(song_string: String):
    var lines = song_string.strip_edges().split("\n")
    var current_track = ""
    for line in lines:
        if line.begins_with("Q:"):
            var tempo_parts = line.substr(2).strip_edges().split("=")
            if tempo_parts.size() == 2:
                beats_per_minute = tempo_parts[1].to_float()
            else:
                beats_per_minute = line.substr(2).to_float()
        elif line.begins_with("K:"):
            key_signature = line.substr(2).strip_edges()
        elif line.begins_with("V:LoopStart="):
            loop_start_beat = line.substr(12).to_float()
        elif line.begins_with("V:"):
            current_track = line.substr(2).strip_edges()
        elif line.begins_with("%%"):
            if current_track != "":
                var path = line.substr(2).strip_edges()
                instruments_per_track[current_track] = path

func calculate_max_duration(notes_by_track):
    var max_duration = 0.0
    for track in notes_by_track.keys():
        var track_duration = 0.0
        for note in notes_by_track[track]:
            track_duration += note["duration"]
        if track_duration > max_duration:
            max_duration = track_duration
    return max_duration

func get_smallest_duration(notes_by_track):
    var smallest_duration = INF
    for track in notes_by_track.keys():
        for note in notes_by_track[track]:
            if note["duration"] < smallest_duration:
                smallest_duration = note["duration"]
    if smallest_duration == float('inf'):
        smallest_duration = 1.0 # Fallback to 1 if no valid duration found
        print("No valid duration found in notes")  # Debugging print
    # smallest_duration *= 0.25 # Convert to quarter notes
    return smallest_duration

func cache_samples():
    instruments.clear()

    for track in parsed_notes.keys():
        var samples: Array[Sample] = []
        var path = instruments_per_track.get(track, "Music/Instruments/Piano") # Default to "Piano" if not specified
        var directory_path = INSTRUMENT_PATH + path
        var dir = DirAccess.open(directory_path)
        if dir:
            dir.list_dir_begin()
            var file_name = dir.get_next()
            while file_name != "":
                if dir.current_is_dir() == false: # and not file_name.ends_with(".import"):
                    file_name = file_name.replace(".import", "")
                    var note = get_note_from_file_name(file_name)
                    path = directory_path + "/" + file_name
                    var clip = load(path)
                    samples.append(Sample.new(clip, note))
                file_name = dir.get_next()
            dir.list_dir_end()
            samples.sort_custom(_compare_samples)
            instruments[track] = samples
            if debug:
                print("Loaded samples for track ", track, ": ", samples)  # Debugging print
        else:
            print("Directory not found: " + directory_path)

func get_sample_for_note(samples: Array[Sample], note: String, audio_stream_player: AudioStreamPlayer) -> AudioStream:
    if note == "z":
        return null  # Return null for rests

    var note_midi = note_to_midi(note)
    if note_midi == -1:
        print("Invalid note: ", note)  # Debugging print
        return null  # Invalid note

    if samples.size() == 0:
        return null  # No samples found

    # Find the sample with the closest start note to the requested note 
    var closest_sample = get_closest_sample(samples, note)

    var closest_sample_midi = note_to_midi(closest_sample.start_note)
    audio_stream_player.pitch_scale = pow(2, (note_midi - closest_sample_midi) / 12.0)

    print("Note: ", note, " Closest sample: ", closest_sample.start_note, " Pitch scale: ", audio_stream_player.pitch_scale)  # Debugging print

    return closest_sample.clip

func get_closest_sample(samples: Array[Sample], note: String) -> Sample:
    var note_midi = note_to_midi(note)
    if note_midi == -1:
        print("Invalid note: ", note)  # Debugging print
        return null  # Invalid note

    if samples.size() == 0:
        return null  # No samples found

    # Find the sample with the closest start note to the requested note 
    var closest_sample = samples[-1]
    var closest_sample_midi = note_to_midi(closest_sample.start_note)
    var closest_sample_diff = abs(note_midi - closest_sample_midi)

    for sample in samples:
        var sample_midi = note_to_midi(sample.start_note)
        var sample_diff = abs(note_midi - sample_midi)
        if sample_diff < closest_sample_diff:
            closest_sample = sample
            closest_sample_diff = sample_diff

    return closest_sample

func get_note_from_file_name(file_name: String) -> String:
    # Remove the file extension
    var note = file_name.split(".")[0]
    
    # Parse the octave from the note
    var octave_string = ""
    var i = note.length() - 1
    while i >= 0 and note[i].is_valid_int():
        octave_string = note[i] + octave_string
        i -= 1
    if octave_string == "":
        octave_string = "4" # Default to middle octave if not specified
    var octave = int(octave_string)

    # Remove the octave from the note
    note = note.replace(octave_string, "")
    
    # Convert the octave to the ABC notation
    var abc_note = note
    if octave < 4:
        for o in range(4 - octave):
            abc_note = abc_note.to_lower() + ","
    elif octave > 4:
        for o in range(octave - 4):
            abc_note = abc_note.to_upper() + "'"

    return abc_note

func _compare_samples(a: Sample, b: Sample) -> bool:
    return is_note_lower_than(a.start_note, b.start_note)

func is_note_lower_than(note1: String, note2: String) -> bool:
    var note1_midi = note_to_midi(note1)
    var note2_midi = note_to_midi(note2)
    return note1_midi < note2_midi

func note_to_midi(note: String) -> int:
    var note_base: Dictionary = {"C": 0, "D": 2, "E": 4, "F": 5, "G": 7, "A": 9, "B": 11}
    var note_value = 0
    var octave = 4 # Default to middle octave if not specified
    var accidental = 0
    var note_letter = ""
    
    var i = 0

    if note.length() == 0:
        return -1 # Invalid note

    # Check for accidental
    while i < note.length() and note[i] in ABCParser.ABC_ACCIDENTALS:
        if note[i] == "^":
            accidental += 1
        elif note[i] == "_":
            accidental -= 1
        elif note[i] == "=":
            accidental = 0
        i += 1

    # Get the note letter
    if i < note.length():
        note_letter = note[i].to_upper()
        i += 1
    else:
        return -1 # Invalid note

    # Check for octave modifiers
    while i < note.length():
        if note[i] == "'":
            octave += 1
        elif note[i] == ",":
            octave -= 1
        i += 1

    # Calculate the MIDI note value
    note_value = note_base.get(note_letter, -1)
    if note_value == -1:
        return -1 # Invalid note letter

    note_value += accidental
    return (octave + 1) * 12 + note_value

func apply_key_signature():
    var key_accidentals = get_key_signature_accidentals(key_signature)
    for track in parsed_notes.keys():
        for note in parsed_notes[track]:
            if note["pitch"] in key_accidentals:
                note["pitch"] = key_accidentals[note["pitch"]]

func get_key_signature_accidentals(key: String) -> Dictionary:
    var accidentals = {}
    match key:
        "C": accidentals = {}
        "G": accidentals = {"F": "^F"}
        "D": accidentals = {"F": "^F", "C": "^C"}
        "A": accidentals = {"F": "^F", "C": "^C", "G": "^G"}
        "E": accidentals = {"F": "^F", "C": "^C", "G": "^G", "D": "^D"}
        "B": accidentals = {"F": "^F", "C": "^C", "G": "^G", "D": "^D", "A": "^A"}
        "F#": accidentals = {"F": "^F", "C": "^C", "G": "^G", "D": "^D", "A": "^A", "E": "^E"}
        "C#": accidentals = {"F": "^F", "C": "^C", "G": "^G", "D": "^D", "A": "^A", "E": "^E", "B": "^B"}
        "F": accidentals = {"B": "_B"}
        "Bb": accidentals = {"B": "_B", "E": "_E"}
        "Eb": accidentals = {"B": "_B", "E": "_E", "A": "_A"}
        "Ab": accidentals = {"B": "_B", "E": "_E", "A": "_A", "D": "_D"}
        "Db": accidentals = {"B": "_B", "E": "_E", "A": "_A", "D": "_D", "G": "_G"}
        "Gb": accidentals = {"B": "_B", "E": "_E", "A": "_A", "D": "_D", "G": "_G", "C": "_C"}
        "Cb": accidentals = {"B": "_B", "E": "_E", "A": "_A", "D": "_D", "G": "_G", "C": "_C", "F": "_F"}
        _: accidentals = {} # Default to no accidentals for unknown keys
    return accidentals

func play_notes(notes_by_track, max_duration, smallest_duration, loop):
    is_playing = true
    var note_events = []
    for track in notes_by_track.keys():
        var track_notes = []
        var current_note_time = 0.0
        for note in notes_by_track[track]:
            var note_start_time = current_note_time
            var first_event = true
            while note_start_time < current_note_time + note["duration"]:
                if first_event:
                    track_notes.append({"start_time": note_start_time, "note": note, "track": track, "play": true})
                    first_event = false
                else:
                    track_notes.append({"start_time": note_start_time, "note": {"pitch": "z", "duration": smallest_duration}, "track": track, "play": false})
                note_start_time += smallest_duration
            current_note_time += note["duration"]
        note_events.append(track_notes)

    if debug:
        print("Note events generated")
    var index = 0
    var max_index = int(max_duration / smallest_duration)

    while is_playing:
        if debug:
            print("Playback loop start")
        var start_time = Time.get_ticks_msec()
        var current_events = []
        for track_notes in note_events:
            if index < track_notes.size():
                var event = track_notes[index]
                current_events.append(event)
                if event["play"]:
                    if debug:
                        print("Playing event: ", event)  # Debugging print
                    play_note_async(event["note"], event["track"])

        await get_tree().create_timer(smallest_duration * (60.0 / beats_per_minute)).timeout
        index += 1

        if not loop and index >= max_index:
            is_playing = false
            break

        if loop and index >= max_index:
            index = 0
            # Skip to the loop start time
            var loop_start_index = int(loop_start_beat * (60.0 / beats_per_minute) / smallest_duration)
            index = loop_start_index

        var end_time = Time.get_ticks_msec()
        var elapsed_time = end_time - start_time
        if debug:
            print("Elapsed time for iteration: ", elapsed_time)  # Debugging print
        if elapsed_time < smallest_duration * (60.0 / beats_per_minute) * 1000:
            await get_tree().create_timer((smallest_duration * (60.0 / beats_per_minute) * 1000 - elapsed_time) / 1000.0).timeout

    if debug:
        print("Playback ended")

func play_note_async(note: Dictionary, track: String) -> void:
    if note["pitch"] == "z":
        return

    var player = AudioStreamPlayer.new()
    add_child(player)
    var sample = get_sample_for_note(instruments[track], note["pitch"], player)
    if sample:
        player.stream = sample
        if debug:
            print("Playing note: ", note["pitch"], " with pitch scale: ", player.pitch_scale, " for duration: ", note["duration"])
        player.play()
        await get_tree().create_timer(note["duration"] * (60.0 / beats_per_minute)).timeout
        player.stop()
    else:
        print("Sample not found for note: " + note["pitch"] + " in track: " + track)
    player.queue_free()
