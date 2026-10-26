import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';

/// Shown when the flavor has no Firebase options (dev machines without config).
class NotConfiguredPage extends StatelessWidget {
  const NotConfiguredPage({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return ErpScreen(
      body: ErpEmptyState(
        icon: ErpIcons.alertTriangle,
        title: l10n.notConfiguredTitle,
        message: l10n.notConfiguredBody,
      ),
    );
  }
}
