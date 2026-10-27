import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';

/// Square bordered card: surface, hairline border, zero radius, padding.
/// [title] (optional) sits above [children] in the title style.
class ErpCard extends StatelessWidget {
  const ErpCard({required this.children, this.title, super.key});

  final List<Widget> children;
  final String? title;

  @override
  Widget build(BuildContext context) {
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
            if (title != null) ...[
              Text(title!, style: text.titleMedium),
              SizedBox(height: t.space2),
            ],
            ...children,
          ],
        ),
      ),
    );
  }
}
