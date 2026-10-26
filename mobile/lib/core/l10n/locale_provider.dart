import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

/// App language, Thai by default; initialised from the user's preferred
/// locale on login. Changes stay local: `PUT /me/profile` cannot change only
/// the locale (it also requires displayName and resends phone/avatar).
class LocaleController extends Notifier<Locale> {
  @override
  Locale build() => const Locale('th');

  Locale get locale => state;
  set locale(Locale value) => state = value;
}

final localeProvider = NotifierProvider<LocaleController, Locale>(
  LocaleController.new,
);
