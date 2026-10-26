import 'package:flutter/material.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/app_theme.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_confirm_dialog.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_mono_loading.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/core/ui/erp_select.dart';
import 'package:tan_erp_field/core/ui/erp_text_field.dart';

/// Dev-flavor only: every core widget under light/dark, normal/compact, 100-130%.
class UiGalleryPage extends StatefulWidget {
  const UiGalleryPage({super.key});

  @override
  State<UiGalleryPage> createState() => _UiGalleryPageState();
}

class _UiGalleryPageState extends State<UiGalleryPage> {
  final _field = FormControl<String>(validators: [Validators.required]);
  Brightness _mode = Brightness.light;
  ErpDensity _density = ErpDensity.normal;
  double _scale = 1;
  String? _choice;

  @override
  void dispose() {
    _field.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    // The gallery previews a theme independent of the app setting.
    final preview = buildTheme(
      mode: _mode,
      density: _density,
      textScale: _scale,
    );
    return Theme(
      data: preview,
      child: Builder(
        builder: (context) {
          final t = context.tokens;
          return ErpScreen(
            title: l10n.galleryTitle,
            body: ColoredBox(
              color: t.canvas,
              child: ListView(
                padding: EdgeInsets.all(t.space4),
                children: [
                  ErpSelect<Brightness>(
                    label: l10n.themeMode,
                    placeholder: l10n.commonSelect,
                    value: _mode,
                    options: [
                      ErpOption(
                        value: Brightness.light,
                        label: l10n.themeModeLight,
                      ),
                      ErpOption(
                        value: Brightness.dark,
                        label: l10n.themeModeDark,
                      ),
                    ],
                    onChanged: (v) => setState(() => _mode = v),
                  ),
                  SizedBox(height: t.space3),
                  ErpSelect<ErpDensity>(
                    label: l10n.themeDensity,
                    placeholder: l10n.commonSelect,
                    value: _density,
                    options: [
                      ErpOption(
                        value: ErpDensity.normal,
                        label: l10n.themeDensityNormal,
                      ),
                      ErpOption(
                        value: ErpDensity.compact,
                        label: l10n.themeDensityCompact,
                      ),
                    ],
                    onChanged: (v) => setState(() => _density = v),
                  ),
                  SizedBox(height: t.space3),
                  ErpSelect<double>(
                    label: l10n.themeTextScale,
                    placeholder: l10n.commonSelect,
                    value: _scale,
                    options: [
                      for (final s in const [1.0, 1.3])
                        ErpOption(
                          value: s,
                          label: l10n.themeTextScaleValue((s * 100).round()),
                        ),
                    ],
                    onChanged: (v) => setState(() => _scale = v),
                  ),
                  SizedBox(height: t.space6),
                  ErpButton(label: l10n.galleryPrimaryButton, onPressed: () {}),
                  SizedBox(height: t.space2),
                  ErpButton(
                    label: l10n.galleryOutlineButton,
                    variant: ErpButtonVariant.outline,
                    onPressed: () {},
                  ),
                  SizedBox(height: t.space2),
                  ErpButton(
                    label: l10n.galleryGhostButton,
                    variant: ErpButtonVariant.ghost,
                    onPressed: () {},
                  ),
                  SizedBox(height: t.space2),
                  ErpButton(
                    label: l10n.galleryLoadingButton,
                    isLoading: true,
                    onPressed: () {},
                  ),
                  SizedBox(height: t.space6),
                  ErpTextField(
                    control: _field,
                    label: l10n.galleryTextField,
                    placeholder: l10n.galleryTextFieldPlaceholder,
                    validationMessages: {
                      ValidationMessage.required: (_) =>
                          l10n.commonFieldRequired,
                    },
                  ),
                  SizedBox(height: t.space4),
                  ErpSelect<String>(
                    label: l10n.gallerySelect,
                    placeholder: l10n.commonSelect,
                    value: _choice,
                    options: const [
                      ErpOption(value: 'a', label: 'A'),
                      ErpOption(value: 'b', label: 'B'),
                    ],
                    onChanged: (v) => setState(() => _choice = v),
                  ),
                  SizedBox(height: t.space6),
                  ErpAlert(message: l10n.galleryAlertInfo),
                  SizedBox(height: t.space2),
                  ErpAlert(
                    message: l10n.galleryAlertSuccess,
                    severity: ErpAlertSeverity.success,
                  ),
                  SizedBox(height: t.space2),
                  ErpAlert(
                    message: l10n.galleryAlertWarning,
                    severity: ErpAlertSeverity.warning,
                  ),
                  SizedBox(height: t.space2),
                  ErpAlert(
                    message: l10n.galleryAlertDanger,
                    severity: ErpAlertSeverity.danger,
                  ),
                  SizedBox(height: t.space6),
                  ErpEmptyState(
                    title: l10n.galleryEmptyTitle,
                    message: l10n.galleryEmptyMessage,
                    actionLabel: l10n.galleryEmptyAction,
                    onAction: () {},
                  ),
                  const ErpMonoLoading(),
                  SizedBox(height: t.space4),
                  ErpButton(
                    label: l10n.galleryConfirmOpen,
                    variant: ErpButtonVariant.outline,
                    onPressed: () => showErpConfirmDialog(
                      context,
                      title: l10n.galleryConfirmOpen,
                      message: l10n.galleryConfirmMessage,
                      confirmLabel: l10n.commonConfirm,
                      cancelLabel: l10n.commonCancel,
                      onConfirm: () =>
                          Future<void>.delayed(const Duration(seconds: 1)),
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}
