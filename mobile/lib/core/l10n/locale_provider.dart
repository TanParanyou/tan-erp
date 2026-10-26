import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

// TODO(A5): initialise from the user's PreferredLocale once /me is typed.
/// App language, Thai by default; a local setting only until then.
class LocaleController extends Notifier<Locale> {
  @override
  Locale build() => const Locale('th');

  Locale get locale => state;
  set locale(Locale value) => state = value;
}

final localeProvider = NotifierProvider<LocaleController, Locale>(
  LocaleController.new,
);
