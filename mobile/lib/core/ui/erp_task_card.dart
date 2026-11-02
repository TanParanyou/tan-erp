import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';

/// Compact tappable work-item card: a status colour strip on the left, a
/// status icon (named for screen readers by [statusLabel]) instead of a text
/// badge, three short lines, and a chevron. At most 3 lines (design.md 8.2).
class ErpTaskCard extends StatelessWidget {
  const ErpTaskCard({
    required this.title,
    required this.subtitle,
    required this.meta,
    required this.statusIcon,
    required this.statusColor,
    required this.statusLabel,
    required this.onTap,
    this.caption,
    this.selected = false,
    super.key,
  });

  final String title;
  final String subtitle;

  /// Optional muted line with a place marker (e.g. the site name).
  final String? caption;

  /// Bottom muted line (schedule or document number).
  final String meta;
  final ErpIcons statusIcon;
  final Color statusColor;
  final String statusLabel;
  final VoidCallback onTap;

  /// Highlights the card as the one open in a master-detail pane.
  final bool selected;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    return InkWell(
      onTap: onTap,
      child: DecoratedBox(
        decoration: BoxDecoration(
          color: selected ? t.actionLight : t.surface,
          borderRadius: t.corner,
          border: Border.all(
            color: selected ? t.action : t.border,
            width: t.borderWidth,
          ),
        ),
        child: IntrinsicHeight(
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              ColoredBox(
                color: statusColor,
                child: SizedBox(width: t.space1),
              ),
              Expanded(
                child: Padding(
                  padding: EdgeInsets.all(t.space3),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Expanded(
                            child: Text(
                              title,
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                              style: text.titleMedium,
                            ),
                          ),
                          SizedBox(width: t.space2),
                          Semantics(
                            label: statusLabel,
                            child: ErpIcon(
                              statusIcon,
                              color: statusColor,
                              size: 22,
                            ),
                          ),
                        ],
                      ),
                      SizedBox(height: t.space1),
                      Text(
                        subtitle,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: text.bodyMedium?.copyWith(color: t.textBody),
                      ),
                      if (caption != null)
                        Row(
                          children: [
                            ErpIcon(
                              ErpIcons.mapPin,
                              color: t.textMuted,
                              size: 16,
                            ),
                            SizedBox(width: t.space1),
                            Expanded(
                              child: Text(
                                caption!,
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                                style: text.bodySmall?.copyWith(
                                  color: t.textMuted,
                                ),
                              ),
                            ),
                          ],
                        ),
                      SizedBox(height: t.space2),
                      Row(
                        children: [
                          Expanded(
                            child: Text(
                              meta,
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                              style: text.bodySmall?.copyWith(
                                color: t.textMuted,
                              ),
                            ),
                          ),
                          ErpIcon(
                            ErpIcons.chevronRight,
                            color: t.textMuted,
                            size: 20,
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
