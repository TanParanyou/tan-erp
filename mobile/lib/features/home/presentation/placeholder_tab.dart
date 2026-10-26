import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';

/// Tabs that are not built yet (today, survey, scan, notifications).
class PlaceholderTab extends StatelessWidget {
  const PlaceholderTab({required this.icon, super.key});

  final ErpIcons icon;

  @override
  Widget build(BuildContext context) => ErpEmptyState(
    icon: icon,
    title: context.l10n.inDevelopmentTitle,
    message: context.l10n.inDevelopmentBody,
  );
}
