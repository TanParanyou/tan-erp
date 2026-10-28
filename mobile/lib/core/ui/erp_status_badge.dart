import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';

enum ErpBadgeTone { neutral, info, success, warning, danger }

/// Sharp architectural status badge using semantic tokens (design.md 7.2, 8.1).
class ErpStatusBadge extends StatelessWidget {
  const ErpStatusBadge({
    required this.label,
    this.tone = ErpBadgeTone.neutral,
    super.key,
  });

  final String label;
  final ErpBadgeTone tone;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final (bg, fg, border) = switch (tone) {
      ErpBadgeTone.neutral => (t.surfaceMuted, t.textMain, t.border),
      ErpBadgeTone.info => (t.infoBg, t.info, t.border),
      ErpBadgeTone.success => (t.successBg, t.success, t.successBorder),
      ErpBadgeTone.warning => (t.warningBg, t.warning, t.warningBorder),
      ErpBadgeTone.danger => (t.dangerBg, t.danger, t.dangerBorder),
    };

    return DecoratedBox(
      decoration: BoxDecoration(
        color: bg,
        borderRadius: t.corner,
        border: Border.all(color: border, width: t.borderWidth),
      ),
      child: Padding(
        padding: EdgeInsets.symmetric(
          horizontal: t.space2,
          vertical: t.space1,
        ),
        child: Text(
          label,
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
            color: fg,
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
    );
  }
}
