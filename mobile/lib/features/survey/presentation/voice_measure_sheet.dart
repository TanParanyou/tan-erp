import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:speech_to_text/speech_recognition_error.dart';
import 'package:speech_to_text/speech_recognition_result.dart';
import 'package:speech_to_text/speech_to_text.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_measure_display.dart';
import 'package:tan_erp_field/features/survey/domain/spoken_number.dart';

/// Listens for a spoken measurement and returns the value the user confirms,
/// in [unitLabel]'s unit. Null when cancelled.
Future<double?> showVoiceMeasureSheet(
  BuildContext context, {
  required String title,
  required String unitLabel,
}) {
  final t = context.tokens;
  return showModalBottomSheet<double>(
    context: context,
    isScrollControlled: true,
    backgroundColor: t.surface,
    shape: RoundedRectangleBorder(borderRadius: t.corner),
    builder: (_) => _VoiceMeasureSheet(title: title, unitLabel: unitLabel),
  );
}

class _VoiceMeasureSheet extends StatefulWidget {
  const _VoiceMeasureSheet({required this.title, required this.unitLabel});

  final String title;
  final String unitLabel;

  @override
  State<_VoiceMeasureSheet> createState() => _VoiceMeasureSheetState();
}

class _VoiceMeasureSheetState extends State<_VoiceMeasureSheet> {
  final _speech = SpeechToText();
  bool _available = true;
  bool _listening = false;
  String _heard = '';
  double? _value;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => unawaited(_listen()));
  }

  @override
  void dispose() {
    unawaited(_speech.cancel());
    super.dispose();
  }

  Future<void> _listen() async {
    // Asks for microphone and speech permission on first use.
    final ok = await _speech.initialize(onError: _onError, onStatus: _onStatus);
    if (!mounted) return;
    if (!ok) {
      setState(() => _available = false);
      return;
    }
    final thai = Localizations.localeOf(context).languageCode == 'th';
    setState(() {
      _listening = true;
      _heard = '';
      _value = null;
    });
    await _speech.listen(
      onResult: _onResult,
      listenOptions: SpeechListenOptions(
        localeId: thai ? 'th_TH' : 'en_US',
        listenFor: const Duration(seconds: 15),
        pauseFor: const Duration(seconds: 3),
      ),
    );
  }

  void _onResult(SpeechRecognitionResult result) {
    if (!mounted) return;
    setState(() {
      _heard = result.recognizedWords;
      _value = parseSpokenNumber(_heard);
    });
  }

  void _onStatus(String status) {
    if (!mounted) return;
    if (status == SpeechToText.doneStatus ||
        status == SpeechToText.notListeningStatus) {
      setState(() => _listening = false);
    }
  }

  void _onError(SpeechRecognitionError error) {
    if (mounted) setState(() => _listening = false);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final fmt = NumberFormat(
      '#,##0.##',
      Localizations.localeOf(context).toLanguageTag(),
    );
    final value = _value;
    final notUnderstood = !_listening && _heard.isNotEmpty && value == null;

    return Padding(
      padding: EdgeInsets.all(t.space4),
      child: SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(widget.title, style: text.titleMedium),
            SizedBox(height: t.space3),
            if (!_available)
              ErpAlert(
                message: l10n.voiceMeasureUnavailable,
                severity: ErpAlertSeverity.danger,
              )
            else ...[
              Text(
                _listening ? l10n.voiceMeasureListening : l10n.voiceMeasureHint,
                style: text.bodyMedium?.copyWith(color: t.textMuted),
              ),
              SizedBox(height: t.space3),
              ErpCard(
                children: [
                  ErpMeasureDisplay(
                    label: _heard.isEmpty ? l10n.voiceMeasureHint : _heard,
                    valueText: value == null
                        ? '-'
                        : '${fmt.format(value)} ${widget.unitLabel}',
                  ),
                ],
              ),
              if (notUnderstood) ...[
                SizedBox(height: t.space3),
                ErpAlert(message: l10n.voiceMeasureNotUnderstood),
              ],
              SizedBox(height: t.space4),
              ErpButton(
                label: l10n.voiceMeasureUse,
                onPressed: value == null || value <= 0
                    ? null
                    : () => Navigator.of(context).pop(value),
              ),
              SizedBox(height: t.space2),
              ErpButton(
                label: l10n.voiceMeasureRetry,
                variant: ErpButtonVariant.outline,
                isLoading: _listening,
                onPressed: _listening ? null : () => unawaited(_listen()),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
