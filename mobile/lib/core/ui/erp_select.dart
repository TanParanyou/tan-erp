import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';

class ErpOption<T> {
  const ErpOption({required this.value, required this.label, this.subtitle});

  final T value;
  final String label;
  final String? subtitle;
}

/// Select that opens a bottom sheet (design.md 8.2) instead of a dropdown.
class ErpSelect<T> extends StatelessWidget {
  const ErpSelect({
    required this.label,
    required this.placeholder,
    required this.options,
    required this.onChanged,
    this.value,
    this.errorText,
    super.key,
  });

  final String label;
  final String placeholder;
  final List<ErpOption<T>> options;
  final T? value;
  final ValueChanged<T> onChanged;
  final String? errorText;

  Future<void> _open(BuildContext context) async {
    final t = context.tokens;
    final picked = await showModalBottomSheet<T>(
      context: context,
      showDragHandle: false,
      builder: (sheetContext) => SafeArea(
        child: ListView(
          shrinkWrap: true,
          children: [
            for (final o in options)
              InkWell(
                onTap: () => Navigator.of(sheetContext).pop(o.value),
                child: ConstrainedBox(
                  constraints: BoxConstraints(minHeight: t.tapTarget),
                  child: Padding(
                    padding: EdgeInsets.symmetric(
                      horizontal: t.space4,
                      vertical: t.space3,
                    ),
                    child: Row(
                      children: [
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                o.label,
                                style: Theme.of(sheetContext)
                                    .textTheme
                                    .titleMedium,
                              ),
                              if (o.subtitle != null)
                                Text(
                                  o.subtitle!,
                                  style: Theme.of(sheetContext)
                                      .textTheme
                                      .bodySmall,
                                ),
                            ],
                          ),
                        ),
                        if (o.value == value)
                          ErpIcon(ErpIcons.check, color: t.action),
                      ],
                    ),
                  ),
                ),
              ),
          ],
        ),
      ),
    );
    if (picked != null) onChanged(picked);
  }

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final selected = options.where((o) => o.value == value).firstOrNull;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: text.labelLarge),
        SizedBox(height: t.space1),
        Semantics(
          button: true,
          label: label,
          child: InkWell(
            onTap: () => _open(context),
            child: DecoratedBox(
              decoration: BoxDecoration(
                color: t.surface,
                borderRadius: t.corner,
                border: Border.all(
                  color: errorText == null ? t.border : t.danger,
                  width: t.borderWidth,
                ),
              ),
              child: ConstrainedBox(
                constraints: BoxConstraints(minHeight: t.tapTarget),
                child: Padding(
                  padding: EdgeInsets.symmetric(horizontal: t.space3),
                  child: Row(
                    children: [
                      Expanded(
                        child: Text(
                          selected?.label ?? placeholder,
                          style: selected == null
                              ? text.bodyMedium?.copyWith(color: t.textSubtle)
                              : text.bodyMedium,
                        ),
                      ),
                      ErpIcon(ErpIcons.chevronDown, color: t.textMuted),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ),
        if (errorText != null)
          Padding(
            padding: EdgeInsets.only(top: t.space1),
            child: Text(
              errorText!,
              style: text.bodySmall?.copyWith(color: t.danger),
            ),
          ),
      ],
    );
  }
}
