import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/features/app_update/data/version_policy_repository.dart';

class UpdateRequiredPage extends ConsumerWidget {
  const UpdateRequiredPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final requirement = ref.watch(updateRequirementProvider);
    final l10n = context.l10n;
    return ErpScreen(
      body: ErpEmptyState(
        icon: ErpIcons.alertTriangle,
        title: l10n.forceUpdateTitle,
        message: requirement == null
            ? null
            : '${l10n.forceUpdateBody(requirement.minimum)}\n'
                  '${l10n.forceUpdateCurrent(requirement.current)}',
      ),
    );
  }
}
