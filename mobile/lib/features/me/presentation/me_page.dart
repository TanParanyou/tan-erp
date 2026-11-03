import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:tan_erp_field/core/auth/session_controller.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/l10n/failure_messages.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/l10n/locale_provider.dart';
import 'package:tan_erp_field/core/notifications/push_notification_service.dart';
import 'package:tan_erp_field/core/router/routes.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/theme/theme_settings.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_confirm_dialog.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_mono_loading.dart';
import 'package:tan_erp_field/core/ui/erp_page_padding.dart';
import 'package:tan_erp_field/core/ui/erp_select.dart';
import 'package:tan_erp_field/core/ui/erp_setting_row.dart';
import 'package:tan_erp_field/features/app_update/data/version_policy_repository.dart';
import 'package:tan_erp_field/features/me/presentation/preferences_controller.dart';

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
    final save = ref.watch(preferencesSaveProvider);
    final saver = ref.read(preferencesSaveProvider.notifier);
    final version = ref.watch(appVersionProvider).value ?? '';
    final membership = session.membership;
    final pushState = ref.watch(pushNotificationProvider);

    Widget info(String label, String value) => Padding(
      padding: EdgeInsets.only(bottom: t.space3),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(label, style: text.bodySmall?.copyWith(color: t.textMuted)),
          Text(value, style: text.bodyLarge),
        ],
      ),
    );

    return ListView(
      padding: ErpPagePadding.of(context),
      children: [
        Text(l10n.meTitle, style: text.titleLarge),
        SizedBox(height: t.space4),
        ErpCard(
          children: [
            Row(
              children: [
                Container(
                  width: t.tapTarget,
                  height: t.tapTarget,
                  alignment: Alignment.center,
                  color: t.action,
                  child: Text(
                    session.user.displayName.characters.first,
                    style: text.titleMedium?.copyWith(color: t.onAction),
                  ),
                ),
                SizedBox(width: t.space3),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(session.user.displayName, style: text.titleMedium),
                      Text(
                        session.user.email,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: text.bodySmall?.copyWith(color: t.textMuted),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            SizedBox(height: t.space3),
            Row(
              children: [
                ErpIcon(ErpIcons.mapPin, color: t.textMuted, size: 20),
                SizedBox(width: t.space2),
                Expanded(
                  child: Text(
                    membership.branchName ?? l10n.membershipAllBranches,
                    style: text.bodyMedium,
                  ),
                ),
              ],
            ),
            Padding(
              padding: EdgeInsets.only(top: t.space1),
              child: Text(
                membership.organizationName,
                style: text.bodySmall?.copyWith(color: t.textMuted),
              ),
            ),
            if (session.user.memberships.length > 1) ...[
              SizedBox(height: t.space3),
              ErpButton(
                label: l10n.meSwitchBranch,
                variant: ErpButtonVariant.outline,
                onPressed: () =>
                    ref.read(sessionProvider.notifier).switchBranch(),
              ),
            ],
          ],
        ),
        SizedBox(height: t.space4),
        if (save.failure != null) ...[
          ErpAlert(
            message: failureMessage(l10n, save.failure!),
            severity: ErpAlertSeverity.danger,
          ),
          SizedBox(height: t.space4),
        ],
        DecoratedBox(
          decoration: BoxDecoration(
            color: t.surface,
            borderRadius: t.corner,
            border: Border.all(color: t.border, width: t.borderWidth),
          ),
          child: Column(
            children: [
              ErpSettingRow<String>(
                icon: ErpIcons.globe,
                label: l10n.meLanguage,
                value: ref.watch(localeProvider).languageCode,
                options: [
                  ErpOption(value: 'th', label: l10n.languageThai),
                  ErpOption(value: 'en', label: l10n.languageEnglish),
                ],
                enabled: !save.isSaving,
                onChanged: (code) => saver.saveLocale(Locale(code)),
              ),
              Divider(height: t.borderWidth, color: t.border),
              ErpSettingRow<ThemeMode>(
                icon: ErpIcons.sun,
                label: l10n.themeMode,
                value: settings.mode,
                options: [
                  ErpOption(
                    value: ThemeMode.system,
                    label: l10n.themeModeSystem,
                  ),
                  ErpOption(value: ThemeMode.light, label: l10n.themeModeLight),
                  ErpOption(value: ThemeMode.dark, label: l10n.themeModeDark),
                ],
                enabled: !save.isSaving,
                onChanged: (v) => saver.save(settings.copyWith(mode: v)),
              ),
              Divider(height: t.borderWidth, color: t.border),
              ErpSettingRow<ErpDensity>(
                icon: ErpIcons.rows,
                label: l10n.themeDensity,
                value: settings.density,
                options: [
                  ErpOption(
                    value: ErpDensity.normal,
                    label: l10n.themeDensityNormal,
                  ),
                  ErpOption(
                    value: ErpDensity.compact,
                    label: l10n.themeDensityCompact,
                  ),
                ],
                enabled: !save.isSaving,
                onChanged: (v) => saver.save(settings.copyWith(density: v)),
              ),
              Divider(height: t.borderWidth, color: t.border),
              ErpSettingRow<double>(
                icon: ErpIcons.typeSize,
                label: l10n.themeTextScale,
                value: settings.textScale,
                options: [
                  for (final s in _scales)
                    ErpOption(
                      value: s,
                      label: l10n.themeTextScaleValue((s * 100).round()),
                    ),
                ],
                enabled: !save.isSaving,
                onChanged: (v) => saver.save(settings.copyWith(textScale: v)),
              ),
            ],
          ),
        ),
        SizedBox(height: t.space6),
        info(l10n.meAppVersion, version),
        if (ref.watch(appConfigProvider).isDev)
          ErpButton(
            label: l10n.meUiGallery,
            variant: ErpButtonVariant.ghost,
            onPressed: () => context.push(AppRoutes.gallery),
          ),
        if (ref.watch(appConfigProvider).isDev)
          ErpButton(
            label: l10n.deviceLabTitle,
            variant: ErpButtonVariant.ghost,
            onPressed: () => context.push(AppRoutes.deviceLab),
          ),
        SizedBox(height: t.space6),
        DecoratedBox(
          decoration: BoxDecoration(
            color: t.surface,
            borderRadius: t.corner,
            border: Border.all(color: t.border, width: t.borderWidth),
          ),
          child: Semantics(
            button: pushState.status == PushPermissionStatus.disabled,
            label: l10n.notificationsTitle,
            value: switch (pushState.status) {
              PushPermissionStatus.enabled => l10n.pushStatusEnabled,
              PushPermissionStatus.disabled => l10n.pushStatusDisabled,
              PushPermissionStatus.unavailable => l10n.pushStatusUnavailable,
            },
            child: InkWell(
              onTap:
                  pushState.status == PushPermissionStatus.disabled &&
                      !pushState.isRequesting
                  ? () => ref
                        .read(pushNotificationProvider.notifier)
                        .requestPermissionAndToken()
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
                      ErpIcon(ErpIcons.bell, color: t.textMuted),
                      SizedBox(width: t.space3),
                      Expanded(
                        child: Text(
                          l10n.notificationsTitle,
                          style: text.bodyLarge,
                        ),
                      ),
                      if (pushState.isRequesting)
                        ErpMonoLoading(size: t.space6)
                      else
                        ErpIcon(
                          switch (pushState.status) {
                            PushPermissionStatus.enabled =>
                              ErpIcons.checkCircle,
                            PushPermissionStatus.disabled =>
                              ErpIcons.alertTriangle,
                            PushPermissionStatus.unavailable =>
                              ErpIcons.cloudOff,
                          },
                          color: switch (pushState.status) {
                            PushPermissionStatus.enabled => t.success,
                            PushPermissionStatus.disabled => t.warning,
                            PushPermissionStatus.unavailable => t.textMuted,
                          },
                        ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ),
        if (ref.watch(appConfigProvider).isDev && pushState.fcmToken != null)
          Padding(
            padding: EdgeInsets.only(top: t.space2),
            child: Text(
              pushState.fcmToken!,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: text.bodySmall?.copyWith(color: t.textMuted),
            ),
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
