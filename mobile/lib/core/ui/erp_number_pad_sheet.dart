import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_measure_display.dart';

/// Opens the custom 56dp field number pad bottom sheet (design.md 8.2:
/// "ป้อนเร็ว: แป้นตัวเลขของแอปเองในจอวัด, แตะช่องแล้วกดแป้นแรกแทนค่าเดิม").
Future<double?> showErpNumberPadSheet(
  BuildContext context, {
  required String title,
  required String unitLabel,
  double? initialValue,
}) => showModalBottomSheet<double>(
  context: context,
  isScrollControlled: true,
  backgroundColor: context.tokens.surface,
  shape: RoundedRectangleBorder(borderRadius: context.tokens.corner),
  builder: (_) => _ErpNumberPadSheet(
    title: title,
    unitLabel: unitLabel,
    initialValue: initialValue,
  ),
);

class _ErpNumberPadSheet extends StatefulWidget {
  const _ErpNumberPadSheet({
    required this.title,
    required this.unitLabel,
    this.initialValue,
  });

  final String title;
  final String unitLabel;
  final double? initialValue;

  @override
  State<_ErpNumberPadSheet> createState() => _ErpNumberPadSheetState();
}

class _ErpNumberPadSheetState extends State<_ErpNumberPadSheet> {
  late String _buffer;
  bool _replaceOnFirstKey = true;

  @override
  void initState() {
    super.initState();
    final init = widget.initialValue;
    if (init == null || init == 0) {
      _buffer = '0';
    } else {
      _buffer = init == init.roundToDouble()
          ? init.toInt().toString()
          : init.toStringAsFixed(2);
    }
  }

  void _onKey(String key) {
    unawaited(HapticFeedback.selectionClick());
    setState(() {
      if (key == 'C') {
        _buffer = '0';
        _replaceOnFirstKey = false;
        return;
      }
      if (key == 'BS') {
        if (_replaceOnFirstKey || _buffer.length <= 1) {
          _buffer = '0';
          _replaceOnFirstKey = false;
        } else {
          _buffer = _buffer.substring(0, _buffer.length - 1);
        }
        return;
      }
      if (_replaceOnFirstKey) {
        _buffer = key == '.' ? '0.' : key;
        _replaceOnFirstKey = false;
        return;
      }
      if (key == '.') {
        if (_buffer.contains('.')) return;
        _buffer = '$_buffer.';
        return;
      }
      if (_buffer == '0') {
        _buffer = key;
      } else if (_buffer.length < 10) {
        _buffer = '$_buffer$key';
      }
    });
  }

  void _submit() {
    final parsed = double.tryParse(_buffer) ?? 0;
    Navigator.of(context).pop(parsed);
  }

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final l10n = context.l10n;
    final text = Theme.of(context).textTheme;
    const keyHeight = 56.0;

    Widget keyButton(String label, String code, {bool muted = false}) =>
        Expanded(
          child: SizedBox(
            height: keyHeight,
            child: OutlinedButton(
              style: OutlinedButton.styleFrom(
                backgroundColor: muted ? t.surfaceMuted : t.surface,
                foregroundColor: t.textMain,
                side: BorderSide(color: t.border, width: t.borderWidth),
                shape: RoundedRectangleBorder(borderRadius: t.corner),
              ),
              onPressed: () => _onKey(code),
              child: Text(label, style: text.titleMedium),
            ),
          ),
        );

    return SafeArea(
      child: Padding(
        padding: EdgeInsets.all(t.space4),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(widget.title, style: text.titleMedium),
            SizedBox(height: t.space3),
            ErpMeasureDisplay(
              valueText: '$_buffer ${widget.unitLabel}',
            ),
            SizedBox(height: t.space4),
            Row(
              children: [
                keyButton('7', '7'),
                SizedBox(width: t.space2),
                keyButton('8', '8'),
                SizedBox(width: t.space2),
                keyButton('9', '9'),
              ],
            ),
            SizedBox(height: t.space2),
            Row(
              children: [
                keyButton('4', '4'),
                SizedBox(width: t.space2),
                keyButton('5', '5'),
                SizedBox(width: t.space2),
                keyButton('6', '6'),
              ],
            ),
            SizedBox(height: t.space2),
            Row(
              children: [
                keyButton('1', '1'),
                SizedBox(width: t.space2),
                keyButton('2', '2'),
                SizedBox(width: t.space2),
                keyButton('3', '3'),
              ],
            ),
            SizedBox(height: t.space2),
            Row(
              children: [
                keyButton(l10n.numpadClear, 'C', muted: true),
                SizedBox(width: t.space2),
                keyButton('0', '0'),
                SizedBox(width: t.space2),
                keyButton('.', '.'),
                SizedBox(width: t.space2),
                keyButton(l10n.numpadBackspace, 'BS', muted: true),
              ],
            ),
            SizedBox(height: t.space4),
            Row(
              children: [
                Expanded(
                  child: ErpButton(
                    label: l10n.commonCancel,
                    variant: ErpButtonVariant.outline,
                    onPressed: () => Navigator.of(context).pop(),
                  ),
                ),
                SizedBox(width: t.space3),
                Expanded(
                  child: ErpButton(
                    label: l10n.numpadDone,
                    onPressed: _submit,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

void unawaited(Future<void> future) {}
