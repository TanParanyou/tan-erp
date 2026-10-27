import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/geometry/plane_measurement.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_select.dart';
import 'package:tan_erp_field/core/ui/erp_text_field.dart';
import 'package:tan_erp_field/features/device_lab/presentation/photo_area_page.dart';

enum _RefPreset { a4, a3, custom }

/// Device Lab card: picks a photo and reference size, then opens the page.
class PhotoAreaCard extends StatefulWidget {
  const PhotoAreaCard({super.key});

  @override
  State<PhotoAreaCard> createState() => _PhotoAreaCardState();
}

class _PhotoAreaCardState extends State<PhotoAreaCard> {
  final _width = FormControl<String>();
  final _height = FormControl<String>();
  _RefPreset _preset = _RefPreset.a4;
  String? _error;
  bool _busy = false;

  @override
  void dispose() {
    _width.dispose();
    _height.dispose();
    super.dispose();
  }

  ReferenceSizeMm? _size() => switch (_preset) {
    _RefPreset.a4 => const ReferenceSizeMm(widthMm: 210, heightMm: 297),
    _RefPreset.a3 => const ReferenceSizeMm(widthMm: 297, heightMm: 420),
    _RefPreset.custom => _customSize(),
  };

  ReferenceSizeMm? _customSize() {
    final w = double.tryParse(_width.value ?? '');
    final h = double.tryParse(_height.value ?? '');
    return w != null && h != null && w > 0 && h > 0
        ? ReferenceSizeMm(widthMm: w, heightMm: h)
        : null;
  }

  Future<void> _pick(ImageSource source) async {
    final l10n = context.l10n;
    final size = _size();
    if (size == null) {
      setState(() => _error = l10n.photoAreaErrorSize);
      return;
    }
    setState(() {
      _error = null;
      _busy = true;
    });
    final navigator = Navigator.of(context);
    try {
      final file = await ImagePicker().pickImage(source: source);
      if (file == null) return;
      final image = await decodeImageFromList(await file.readAsBytes());
      if (!mounted) {
        image.dispose();
        return;
      }
      await navigator.push<void>(
        MaterialPageRoute(
          builder: (_) => PhotoAreaPage(image: image, referenceSize: size),
        ),
      );
      image.dispose();
    } on Exception {
      if (mounted) setState(() => _error = l10n.photoAreaErrorPick);
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: t.surface,
        borderRadius: t.corner,
        border: Border.all(color: t.border, width: t.borderWidth),
      ),
      child: Padding(
        padding: EdgeInsets.all(t.space4),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(l10n.photoAreaCardTitle, style: text.titleMedium),
            SizedBox(height: t.space2),
            Text(l10n.photoAreaCardBody, style: text.bodyMedium),
            SizedBox(height: t.space4),
            ErpSelect<_RefPreset>(
              label: l10n.photoAreaReferenceLabel,
              placeholder: l10n.photoAreaReferencePlaceholder,
              value: _preset,
              options: [
                ErpOption(value: _RefPreset.a4, label: l10n.photoAreaPresetA4),
                ErpOption(value: _RefPreset.a3, label: l10n.photoAreaPresetA3),
                ErpOption(
                  value: _RefPreset.custom,
                  label: l10n.photoAreaPresetCustom,
                ),
              ],
              onChanged: (v) => setState(() => _preset = v),
            ),
            if (_preset == _RefPreset.custom) ...[
              SizedBox(height: t.space3),
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: ErpTextField(
                      control: _width,
                      label: l10n.photoAreaWidthLabel,
                      placeholder: l10n.photoAreaWidthPlaceholder,
                      keyboardType: TextInputType.number,
                    ),
                  ),
                  SizedBox(width: t.space3),
                  Expanded(
                    child: ErpTextField(
                      control: _height,
                      label: l10n.photoAreaHeightLabel,
                      placeholder: l10n.photoAreaHeightPlaceholder,
                      keyboardType: TextInputType.number,
                    ),
                  ),
                ],
              ),
            ],
            if (_error != null) ...[
              SizedBox(height: t.space3),
              ErpAlert(message: _error!, severity: ErpAlertSeverity.danger),
            ],
            SizedBox(height: t.space4),
            ErpButton(
              label: l10n.photoAreaPickGallery,
              isLoading: _busy,
              onPressed: () => _pick(ImageSource.gallery),
            ),
            SizedBox(height: t.space2),
            ErpButton(
              label: l10n.photoAreaPickCamera,
              variant: ErpButtonVariant.outline,
              onPressed: _busy ? null : () => _pick(ImageSource.camera),
            ),
          ],
        ),
      ),
    );
  }
}
