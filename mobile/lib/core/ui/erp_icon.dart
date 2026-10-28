import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

/// Stroke icons copied from frontend/src/components/common/Icons.tsx.
enum ErpIcons {
  calendar('calendar'),
  mapPin('map_pin'),
  camera('camera'),
  bell('bell'),
  user('user'),
  alertCircle('alert_circle'),
  alertTriangle('alert_triangle'),
  checkCircle('check_circle'),
  info('info'),
  chevronDown('chevron_down'),
  chevronRight('chevron_right'),
  logOut('log_out'),
  check('check'),
  plus('plus'),
  search('search'),
  refresh('refresh'),
  trash('trash'),
  ruler('ruler'),
  box('box'),
  calculator('calculator'),
  cloudOff('cloud_off');

  ErpIcons(this.file);
  final String file;
}

class ErpIcon extends StatelessWidget {
  const ErpIcon(this.icon, {required this.color, this.size = 24, super.key});

  final ErpIcons icon;
  final Color color;
  final double size;

  @override
  Widget build(BuildContext context) => SvgPicture.asset(
    'assets/icons/${icon.file}.svg',
    width: size,
    height: size,
    colorFilter: ColorFilter.mode(color, BlendMode.srcIn),
    excludeFromSemantics: true,
  );
}
