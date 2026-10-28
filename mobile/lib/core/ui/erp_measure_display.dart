import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';

/// Main measured value focal display: 44sp tabular figures with the oak
/// dimension underline (`accentMeasure`, design.md 8.1 & 8.2).
class ErpMeasureDisplay extends StatelessWidget {
  const ErpMeasureDisplay({
    required this.valueText,
    this.label,
    this.subLabel,
    this.onTap,
    super.key,
  });

  final String valueText;
  final String? label;
  final String? subLabel;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final content = Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        if (label != null) ...[
          Text(label!, style: text.bodySmall),
          SizedBox(height: t.space1),
        ],
        DecoratedBox(
          decoration: BoxDecoration(
            border: Border(
              bottom: BorderSide(
                color: t.accentMeasure,
                width: t.borderWidth * 2,
              ),
            ),
          ),
          child: Padding(
            padding: EdgeInsets.only(bottom: t.space1),
            child: Text(valueText, style: text.displayMedium),
          ),
        ),
        if (subLabel != null) ...[
          SizedBox(height: t.space1),
          Text(
            subLabel!,
            style: text.bodySmall?.copyWith(color: t.accentMeasure),
          ),
        ],
      ],
    );

    if (onTap == null) return content;
    return InkWell(
      onTap: onTap,
      child: ConstrainedBox(
        constraints: BoxConstraints(minHeight: t.tapTarget),
        child: content,
      ),
    );
  }
}
