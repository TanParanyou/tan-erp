import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:tan_erp_field/core/auth/session_controller.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/router/routes.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_nav_shell.dart';
import 'package:tan_erp_field/features/app_update/data/version_policy_repository.dart';
import 'package:tan_erp_field/features/app_update/presentation/update_required_page.dart';
import 'package:tan_erp_field/features/auth/presentation/lock_page.dart';
import 'package:tan_erp_field/features/auth/presentation/login_page.dart';
import 'package:tan_erp_field/features/auth/presentation/membership_page.dart';
import 'package:tan_erp_field/features/auth/presentation/not_configured_page.dart';
import 'package:tan_erp_field/features/dev/presentation/ui_gallery_page.dart';
import 'package:tan_erp_field/features/home/presentation/placeholder_tab.dart';
import 'package:tan_erp_field/features/me/presentation/me_page.dart';

/// Route guard: forced update first, then the session state decides.
String? _redirect(Ref ref, GoRouterState state) {
  final at = state.matchedLocation;
  if (ref.read(updateRequirementProvider) != null) {
    return at == AppRoutes.updateRequired ? null : AppRoutes.updateRequired;
  }
  final target = switch (ref.read(sessionProvider)) {
    SessionUnavailable() => AppRoutes.notConfigured,
    SessionSignedOut() => AppRoutes.login,
    SessionLocked() => AppRoutes.unlock,
    SessionSelectMembership() => AppRoutes.membership,
    SessionActive() => null,
  };
  if (target != null) return at == target ? null : target;
  const preAuth = {
    AppRoutes.login,
    AppRoutes.unlock,
    AppRoutes.membership,
    AppRoutes.notConfigured,
    AppRoutes.updateRequired,
    '/',
  };
  return preAuth.contains(at) ? AppRoutes.today : null;
}

final routerProvider = Provider<GoRouter>((ref) {
  final refresh = ValueNotifier<int>(0);
  ref
    ..listen(sessionProvider, (_, _) => refresh.value++)
    ..listen(updateRequirementProvider, (_, _) => refresh.value++)
    ..onDispose(refresh.dispose);

  final isDev = ref.watch(appConfigProvider).isDev;

  GoRoute tab(String path, ErpIcons icon) => GoRoute(
    path: path,
    builder: (_, _) => PlaceholderTab(icon: icon),
  );

  return GoRouter(
    initialLocation: AppRoutes.today,
    refreshListenable: refresh,
    redirect: (_, state) => _redirect(ref, state),
    routes: [
      GoRoute(path: AppRoutes.login, builder: (_, _) => const LoginPage()),
      GoRoute(path: AppRoutes.unlock, builder: (_, _) => const LockPage()),
      GoRoute(
        path: AppRoutes.membership,
        builder: (_, _) => const MembershipPage(),
      ),
      GoRoute(
        path: AppRoutes.notConfigured,
        builder: (_, _) => const NotConfiguredPage(),
      ),
      GoRoute(
        path: AppRoutes.updateRequired,
        builder: (_, _) => const UpdateRequiredPage(),
      ),
      if (isDev)
        GoRoute(
          path: AppRoutes.gallery,
          builder: (_, _) => const UiGalleryPage(),
        ),
      StatefulShellRoute.indexedStack(
        builder: (context, _, shell) {
          final l10n = context.l10n;
          return ErpNavShell(
            selectedIndex: shell.currentIndex,
            onSelected: (i) =>
                shell.goBranch(i, initialLocation: i == shell.currentIndex),
            destinations: [
              ErpNavDestination(icon: ErpIcons.calendar, label: l10n.navToday),
              ErpNavDestination(icon: ErpIcons.mapPin, label: l10n.navSurvey),
              ErpNavDestination(icon: ErpIcons.camera, label: l10n.navScan),
              ErpNavDestination(
                icon: ErpIcons.bell,
                label: l10n.navNotifications,
              ),
              ErpNavDestination(icon: ErpIcons.user, label: l10n.navMe),
            ],
            child: shell,
          );
        },
        branches: [
          StatefulShellBranch(
            routes: [tab(AppRoutes.today, ErpIcons.calendar)],
          ),
          StatefulShellBranch(routes: [tab(AppRoutes.survey, ErpIcons.mapPin)]),
          StatefulShellBranch(routes: [tab(AppRoutes.scan, ErpIcons.camera)]),
          StatefulShellBranch(
            routes: [tab(AppRoutes.notifications, ErpIcons.bell)],
          ),
          StatefulShellBranch(
            routes: [
              GoRoute(path: AppRoutes.me, builder: (_, _) => const MePage()),
            ],
          ),
        ],
      ),
    ],
  );
});
