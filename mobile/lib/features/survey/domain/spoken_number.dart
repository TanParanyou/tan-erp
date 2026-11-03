/// Reads a measurement value out of speech-to-text output.
///
/// Handles digits ("2.5", "2,500", "2 จุด 5") and Thai number words
/// ("สองจุดห้า", "ยี่สิบเอ็ด", "สองร้อยห้าสิบ", "สองเมตรครึ่ง").
/// Unit words and other text are ignored. Returns null when no number is heard.
double? parseSpokenNumber(String text) {
  if (RegExp('[0-9]').hasMatch(text)) {
    final cleaned = text
        .replaceAll('จุด', '.')
        .replaceAll(RegExp(r'[\s,]'), '');
    final match = RegExp(r'\d+(\.\d+)?').firstMatch(cleaned);
    final value = match == null ? null : double.tryParse(match.group(0)!);
    if (value == null) return null;
    return cleaned.contains('ครึ่ง') && match!.group(1) == null
        ? value + 0.5
        : value;
  }

  var total = 0;
  var current = 0;
  var heard = false;
  var inDecimal = false;
  var half = false;
  final decimals = StringBuffer();
  var i = 0;
  while (i < text.length) {
    final token = _tokens.firstWhere(
      (tok) => text.startsWith(tok.$1, i),
      orElse: () => ('', _Kind.none, 0),
    );
    if (token.$2 == _Kind.none) {
      i++;
      continue;
    }
    i += token.$1.length;
    switch (token.$2) {
      case _Kind.digit:
        heard = true;
        if (inDecimal) {
          decimals.write(token.$3);
        } else {
          current = token.$3;
        }
      case _Kind.multiplier:
        heard = true;
        if (!inDecimal) {
          total += (current == 0 ? 1 : current) * token.$3;
          current = 0;
        }
      case _Kind.point:
        inDecimal = true;
      case _Kind.half:
        half = true;
      case _Kind.none:
        break;
    }
  }
  if (!heard && !half) return null;
  final whole = total + current;
  if (decimals.isNotEmpty) return double.parse('$whole.$decimals');
  return whole + (half ? 0.5 : 0);
}

enum _Kind { digit, multiplier, point, half, none }

// Longer words first so "เอ็ด" and "สิบ" are not split by shorter matches.
const _tokens = <(String, _Kind, int)>[
  ('ศูนย์', _Kind.digit, 0),
  ('หนึ่ง', _Kind.digit, 1),
  ('เอ็ด', _Kind.digit, 1),
  ('สอง', _Kind.digit, 2),
  ('ยี่', _Kind.digit, 2),
  ('สาม', _Kind.digit, 3),
  ('สี่', _Kind.digit, 4),
  ('ห้า', _Kind.digit, 5),
  ('หก', _Kind.digit, 6),
  ('เจ็ด', _Kind.digit, 7),
  ('แปด', _Kind.digit, 8),
  ('เก้า', _Kind.digit, 9),
  ('สิบ', _Kind.multiplier, 10),
  ('ร้อย', _Kind.multiplier, 100),
  ('พัน', _Kind.multiplier, 1000),
  ('หมื่น', _Kind.multiplier, 10000),
  ('แสน', _Kind.multiplier, 100000),
  ('จุด', _Kind.point, 0),
  ('ครึ่ง', _Kind.half, 0),
];
