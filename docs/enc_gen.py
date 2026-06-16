keys = list("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") + [
    '!', '"', '#', '$', '%', '&', "'", '(', ')', '-',
    '+', '*', '/', '\\', '=', '^', '~', '|', '`', '@',
    '[', ']', '{', '}', ';', ':', '_', '?', ',', '.', '<', '>']
vals = [
    '?', '.', 'u', ',', '[', '<', '8', 'z', ';', 'y',
    '+', 'J', 'Q', 'K', 'C', 'H', 'T', 'B', 'R', 'F',
    'U', 'S', 'D', 'O', 'Y', 'E', 'I', 'X', 'N', 'L',
    'A', 'W', 'Z', 'V', 'P', 'M', 'G', '%', '1', '|',
    '6', '/', 'c', '"', '{', 'f', '0', 'g', 'h', '*',
    'i', 'x', 'k', 'l', '7', 'n', 'j', '~', 'r', 's',
    '>', '5', 't', 'a', 'v', 'w', '3', '&', '9', '!',
    'p', '#', '$', 'd', "'", '(', ')', '-', '\\', 'q',
    '=', '^', 'm', '`', '4', '@', 'b', ']', 'o', '}',
    'e', '2', ':', '_']
assert len(keys) == 94 and len(vals) == 94, (len(keys), len(vals))
enc = {k: v for k, v in zip(keys, vals)}
dec = {v: k for k, v in zip(keys, vals)}
def E(s): return ''.join(enc.get(c, c) for c in s)
def D(s): return ''.join(dec.get(c, c) for c in s)

print("decrypt existing OPEN_DB   'P||6o6/A|' ->", D('P||6o6/A|'))
print("decrypt existing OPEN_USER 'CH[K'      ->", D('CH[K'))
print("decrypt existing OPEN_PWD  'RYRF[Q'    ->", D('RYRF[Q'))
print("decrypt existing MAIN_DB   'P||6o6\"N%' ->", D('P||6o6"N%'))
print("---- encrypt local values ----")
for k, v in [("OPEN_DB", "localhost:1521/FREEPDB1"), ("OPEN_USER", "TEST_USER"),
             ("OPEN_PWD", "TEST_PWD"), ("MAIN_DB", "localhost:1521/FREEPDB1"),
             ("MAIN_USER", "TEST_USER"), ("MAIN_PWD", "TEST_PWD")]:
    e = E(v)
    print("%-10s=%-28s # roundtrip=%r" % (k, e, D(e)))
