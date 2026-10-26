import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';

enum ErpAlertSeverity { info, success, warning, danger }

class ErpAlert extends StatelessWidget {
  const ErpAlert({
    required this.message,
    this.severity = ErpAlertSeverity.info,
    this.title,
    super.key,
  });

  final String message;
  final String? title;
  final ErpAlertSeverity severity;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final (bg, border, fg, icon) = switch (severity) {
      ErpAlertSeverity.info => (t.infoBg, t.info, t.info, ErpIcons.info),
      ErpAlertSeverity.success => (
        t.successBg,
        t.successBorder,
        t.success,
        ErpIcons.checkCircle,
      ),
      ErpAlertSeverity.warning => (
        t.warningBg,
        t.warningBorder,
        t.warning,
        ErpIcons.alertTriangle,
      ),
      ErpAlertSeverity.danger => (
        t.dangerBg,
        t.dangerBorder,
        t.danger,
        ErpIcons.alertCircle,
      ),
    };
    final text = Theme.of(context).textTheme;
    return Semantics(
      liveRegion: true,
      container: true,
      child: DecoratedBox(
        decoration: BoxDecoration(
          color: bg,
          border: Border.all(color: border, width: t.borderWidth),
          borderRadius: t.corner,
        ),
        child: Padding(
          padding: EdgeInsets.all(t.space3),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              ErpIcon(icon, color: fg),
              SizedBox(width: t.space3),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    if (title != null)
                      Text(title!, style: text.labelLarge?.copyWith(color: fg)),
                    Text(message, style: text.bodyMedium?.copyWith(color: fg)),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
