import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:tan_erp_field/core/auth/session_controller.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/l10n/locale_provider.dart';
import 'package:tan_erp_field/core/router/routes.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/theme/theme_settings.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_confirm_dialog.dart';
import 'package:tan_erp_field/core/ui/erp_select.dart';
import 'package:tan_erp_field/features/app_update/data/version_policy_repository.dart';

class MePage extends ConsumerWidget {
  const MePage({super.key});

  static const _scales = [0.9, 1.0, 1.1, 1.2, 1.3];

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final session = ref.watch(sessionProvider);
    if (session is! SessionActive) return const SizedBox.shrink();
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final settings = ref.watch(themeSettingsProvider);
    final controller = ref.read(themeSettingsProvider.notifier);
    final version = ref.watch(appVersionProvider).value ?? '';
    final membership = session.membership;

    Widget info(String label, String value) => Padding(
      padding: EdgeInsets.only(bottom: t.space3),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(label, style: text.bodySmall),
          Text(value, style: text.bodyLarge),
        ],
      ),
    );

    return ListView(
      padding: EdgeInsets.all(t.space4),
      children: [
        Text(l10n.meTitle, style: text.titleLarge),
        SizedBox(height: t.space4),
        info(l10n.meProfile, session.user.displayName),
        info(l10n.loginEmailLabel, session.user.email),
        info(l10n.meOrganization, membership.organizationName),
        info(
          l10n.meBranch,
          membership.branchName ?? l10n.membershipAllBranches,
        ),
        if (session.user.memberships.length > 1)
          ErpButton(
            label: l10n.meSwitchBranch,
            variant: ErpButtonVariant.outline,
            onPressed: () => ref.read(sessionProvider.notifier).switchBranch(),
          ),
        SizedBox(height: t.space6),
        ErpSelect<String>(
          label: l10n.meLanguage,
          placeholder: l10n.commonSelect,
          value: ref.watch(localeProvider).languageCode,
          options: [
            ErpOption(value: 'th', label: l10n.languageThai),
            ErpOption(value: 'en', label: l10n.languageEnglish),
          ],
          onChanged: (code) =>
              ref.read(localeProvider.notifier).locale = Locale(code),
        ),
        SizedBox(height: t.space4),
        // TODO(A5): persist theme mode/density/text scale via the backend
        // preferences API; these three controls are local-only until then.
        ErpSelect<ThemeMode>(
          label: l10n.themeMode,
          placeholder: l10n.commonSelect,
          value: settings.mode,
          options: [
            ErpOption(value: ThemeMode.system, label: l10n.themeModeSystem),
            ErpOption(value: ThemeMode.light, label: l10n.themeModeLight),
            ErpOption(value: ThemeMode.dark, label: l10n.themeModeDark),
          ],
          onChanged: (v) => controller.settings = settings.copyWith(mode: v),
        ),
        SizedBox(height: t.space4),
        ErpSelect<ErpDensity>(
          label: l10n.themeDensity,
          placeholder: l10n.commonSelect,
          value: settings.density,
          options: [
            ErpOption(value: ErpDensity.normal, label: l10n.themeDensityNormal),
            ErpOption(
              value: ErpDensity.compact,
              label: l10n.themeDensityCompact,
            ),
          ],
          onChanged: (v) => controller.settings = settings.copyWith(density: v),
        ),
        SizedBox(height: t.space4),
        ErpSelect<double>(
          label: l10n.themeTextScale,
          placeholder: l10n.commonSelect,
          value: settings.textScale,
          options: [
            for (final s in _scales)
              ErpOption(
                value: s,
                label: l10n.themeTextScaleValue((s * 100).round()),
              ),
          ],
          onChanged: (v) =>
              controller.settings = settings.copyWith(textScale: v),
        ),
        SizedBox(height: t.space6),
        info(l10n.meAppVersion, version),
        if (ref.watch(appConfigProvider).isDev)
          ErpButton(
            label: l10n.meUiGallery,
            variant: ErpButtonVariant.ghost,
            onPressed: () => context.push(AppRoutes.gallery),
          ),
        SizedBox(height: t.space6),
        ErpButton(
          label: l10n.meLogout,
          variant: ErpButtonVariant.outline,
          onPressed: () => showErpConfirmDialog(
            context,
            title: l10n.logoutConfirmTitle,
            message: l10n.logoutConfirmBody,
            confirmLabel: l10n.meLogout,
            cancelLabel: l10n.commonCancel,
            onConfirm: () => ref.read(sessionProvider.notifier).signOut(),
          ),
        ),
      ],
    );
  }
}
