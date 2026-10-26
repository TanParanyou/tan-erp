import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/l10n/locale_provider.dart';
import 'package:tan_erp_field/core/router/app_router.dart';
import 'package:tan_erp_field/core/theme/app_theme.dart';
import 'package:tan_erp_field/core/theme/theme_settings.dart';

class App extends ConsumerWidget {
  const App({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final settings = ref.watch(themeSettingsProvider);
    ThemeData theme(Brightness mode) => buildTheme(
      mode: mode,
      density: settings.density,
      textScale: settings.textScale,
    );
    return MaterialApp.router(
      onGenerateTitle: (context) => context.l10n.appTitle,
      debugShowCheckedModeBanner: false,
      routerConfig: ref.watch(routerProvider),
      theme: theme(Brightness.light),
      darkTheme: theme(Brightness.dark),
      themeMode: settings.mode,
      locale: ref.watch(localeProvider),
      supportedLocales: AppLocalizations.supportedLocales,
      localizationsDelegates: AppLocalizations.localizationsDelegates,
    );
  }
}
