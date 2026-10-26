import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';

/// Empty screen: short reason plus at most one follow-up action.
class ErpEmptyState extends StatelessWidget {
  const ErpEmptyState({
    required this.title,
    this.message,
    this.icon = ErpIcons.info,
    this.actionLabel,
    this.onAction,
    super.key,
  });

  final String title;
  final String? message;
  final ErpIcons icon;
  final String? actionLabel;
  final VoidCallback? onAction;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    return Center(
      child: Padding(
        padding: EdgeInsets.all(t.space6),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ErpIcon(icon, color: t.textSubtle, size: t.space8),
            SizedBox(height: t.space4),
            Text(title, style: text.titleMedium, textAlign: TextAlign.center),
            if (message != null) ...[
              SizedBox(height: t.space2),
              Text(
                message!,
                style: text.bodyMedium,
                textAlign: TextAlign.center,
              ),
            ],
            if (actionLabel != null) ...[
              SizedBox(height: t.space6),
              ErpButton(label: actionLabel!, onPressed: onAction),
            ],
          ],
        ),
      ),
    );
  }
}
