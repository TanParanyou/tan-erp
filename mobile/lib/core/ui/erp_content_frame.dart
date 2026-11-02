import 'package:flutter/material.dart';

/// Centres page content and caps its width on tablets so lists and forms do
/// not stretch edge to edge. Phones are unaffected (width < [maxWidth]).
class ErpContentFrame extends StatelessWidget {
  const ErpContentFrame({required this.child, super.key});

  static const double maxWidth = 840;

  final Widget child;

  @override
  Widget build(BuildContext context) => Align(
    alignment: Alignment.topCenter,
    child: ConstrainedBox(
      constraints: const BoxConstraints(maxWidth: maxWidth),
      child: child,
    ),
  );
}
