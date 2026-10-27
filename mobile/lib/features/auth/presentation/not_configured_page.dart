import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/router/routes.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';

/// Shown when the flavor has no Firebase options (dev machines without config).
class NotConfiguredPage extends ConsumerWidget {
  const NotConfiguredPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final isDev = ref.watch(appConfigProvider).isDev;
    return ErpScreen(
      body: ErpEmptyState(
        icon: ErpIcons.alertTriangle,
        title: l10n.notConfiguredTitle,
        message: l10n.notConfiguredBody,
        actionLabel: isDev ? l10n.deviceLabTitle : null,
        onAction: isDev ? () => context.push(AppRoutes.deviceLab) : null,
      ),
    );
  }
}
