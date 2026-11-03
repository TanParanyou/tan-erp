import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';

/// Single-row filter strip: one flat segment per option, the selected one in
/// the action colour. Keeps the screen's one primary button unique.
class ErpSegmentedFilter extends StatelessWidget {
  const ErpSegmentedFilter({
    required this.labels,
    required this.selectedIndex,
    required this.onSelected,
    super.key,
  });

  final List<String> labels;
  final int selectedIndex;
  final ValueChanged<int> onSelected;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    return DecoratedBox(
      decoration: BoxDecoration(
        border: Border.all(color: t.borderStrong, width: t.borderWidth),
        borderRadius: t.corner,
      ),
      child: Row(
        children: [
          for (var i = 0; i < labels.length; i++)
            Expanded(
              child: Semantics(
                button: true,
                selected: i == selectedIndex,
                child: InkWell(
                  onTap: () => onSelected(i),
                  child: Container(
                    constraints: BoxConstraints(minHeight: t.tapTarget),
                    alignment: Alignment.center,
                    padding: EdgeInsets.symmetric(horizontal: t.space2),
                    color: i == selectedIndex ? t.action : null,
                    child: Text(
                      labels[i],
                      textAlign: TextAlign.center,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: text.labelLarge?.copyWith(
                        color: i == selectedIndex ? t.onAction : t.textMain,
                        fontWeight: i == selectedIndex
                            ? FontWeight.w600
                            : FontWeight.w400,
                      ),
                    ),
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }
}
