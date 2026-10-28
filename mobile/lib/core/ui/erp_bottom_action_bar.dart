import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';

/// Sticky thumb-zone action bar at the bottom of a screen (design.md 8.2:
/// "1 จอ 1 งาน: ปุ่มหลักปุ่มเดียวต่อจอ เต็มความกว้าง สูง 52dp ติดล่างจอ").
class ErpBottomActionBar extends StatelessWidget {
  const ErpBottomActionBar({
    required this.label,
    required this.onPressed,
    this.isLoading = false,
    this.secondaryLabel,
    this.onSecondaryPressed,
    super.key,
  });

  final String label;
  final VoidCallback? onPressed;
  final bool isLoading;
  final String? secondaryLabel;
  final VoidCallback? onSecondaryPressed;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: t.surface,
        border: Border(
          top: BorderSide(color: t.border, width: t.borderWidth),
        ),
      ),
      child: Padding(
        padding: EdgeInsets.all(t.space3),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (secondaryLabel != null) ...[
              ErpButton(
                label: secondaryLabel!,
                variant: ErpButtonVariant.outline,
                onPressed: isLoading ? null : onSecondaryPressed,
              ),
              SizedBox(height: t.space2),
            ],
            ErpButton(
              label: label,
              isLoading: isLoading,
              onPressed: onPressed,
            ),
          ],
        ),
      ),
    );
  }
}
