import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_mono_loading.dart';

enum ErpButtonVariant { primary, outline, ghost }

/// Central button. Primary is the single 52dp full-width action of a screen;
/// outline/ghost keep the 48dp tap target. [isLoading] disables the button.
class ErpButton extends StatelessWidget {
  const ErpButton({
    required this.label,
    required this.onPressed,
    this.variant = ErpButtonVariant.primary,
    this.isLoading = false,
    this.expand = true,
    super.key,
  });

  final String label;
  final VoidCallback? onPressed;
  final ErpButtonVariant variant;
  final bool isLoading;
  final bool expand;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final height = variant == ErpButtonVariant.primary
        ? t.primaryActionHeight
        : t.tapTarget;
    final shape = RoundedRectangleBorder(borderRadius: t.corner);
    final onTap = isLoading ? null : onPressed;
    final fg = variant == ErpButtonVariant.primary ? t.onAction : t.textMain;
    final child = isLoading
        ? ErpMonoLoading(size: t.space6, color: fg)
        : Text(label, textAlign: TextAlign.center);
    final size = Size(expand ? double.infinity : 0, height);

    return switch (variant) {
      ErpButtonVariant.primary => FilledButton(
        onPressed: onTap,
        style: FilledButton.styleFrom(
          minimumSize: size,
          shape: shape,
          backgroundColor: t.action,
          foregroundColor: t.onAction,
          disabledBackgroundColor: t.surfaceSubtle,
          disabledForegroundColor: t.textSubtle,
        ),
        child: child,
      ),
      ErpButtonVariant.outline => OutlinedButton(
        onPressed: onTap,
        style: OutlinedButton.styleFrom(
          minimumSize: size,
          shape: shape,
          foregroundColor: t.textMain,
          side: BorderSide(color: t.borderStrong, width: t.borderWidth),
        ),
        child: child,
      ),
      ErpButtonVariant.ghost => TextButton(
        onPressed: onTap,
        style: TextButton.styleFrom(
          minimumSize: size,
          shape: shape,
          foregroundColor: t.textMain,
        ),
        child: child,
      ),
    };
  }
}
