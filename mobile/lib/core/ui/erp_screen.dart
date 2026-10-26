import 'package:flutter/material.dart';

/// Standard screen frame: app bar + safe-area body.
class ErpScreen extends StatelessWidget {
  const ErpScreen({required this.body, this.title, this.actions, super.key});

  final String? title;
  final Widget body;
  final List<Widget>? actions;

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: title == null
        ? null
        : AppBar(title: Text(title!), actions: actions),
    body: SafeArea(child: body),
  );
}
