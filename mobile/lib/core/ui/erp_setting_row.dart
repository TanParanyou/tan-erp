import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_select.dart';

/// Icon-led settings row: icon, label, current value, chevron. Tapping opens
/// the shared option bottom sheet (design.md 8.2), so no dropdown field.
class ErpSettingRow<T> extends StatelessWidget {
  const ErpSettingRow({
    required this.icon,
    required this.label,
    required this.options,
    required this.value,
    required this.onChanged,
    this.enabled = true,
    super.key,
  });

  final ErpIcons icon;
  final String label;
  final List<ErpOption<T>> options;
  final T value;
  final ValueChanged<T> onChanged;
  final bool enabled;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final selected = options.where((o) => o.value == value).firstOrNull;
    return Semantics(
      button: true,
      enabled: enabled,
      label: label,
      value: selected?.label,
      child: InkWell(
        onTap: enabled
            ? () async {
                final picked = await showErpOptionSheet<T>(
                  context,
                  options: options,
                  value: value,
                );
                if (picked != null) onChanged(picked);
              }
            : null,
        child: ConstrainedBox(
          constraints: BoxConstraints(minHeight: t.tapTarget),
          child: Padding(
            padding: EdgeInsets.symmetric(
              horizontal: t.space4,
              vertical: t.space2,
            ),
            child: Row(
              children: [
                ErpIcon(icon, color: t.textMuted),
                SizedBox(width: t.space3),
                Expanded(child: Text(label, style: text.bodyLarge)),
                Text(
                  selected?.label ?? '',
                  style: text.bodyMedium?.copyWith(color: t.textMuted),
                ),
                SizedBox(width: t.space2),
                ErpIcon(ErpIcons.chevronRight, color: t.textMuted, size: 20),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
