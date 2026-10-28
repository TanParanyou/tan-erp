import 'dart:async';
import 'dart:math' as math;
import 'dart:ui' as ui;

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import 'package:tan_erp_field/core/geometry/plane_measurement.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_measure_display.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/features/device_lab/presentation/photo_area_controller.dart';

const double _loupeSize = 120;
const double _loupeGap = 28;

/// Full-screen photo measurement: tap reference corners, then area corners.
class PhotoAreaPage extends ConsumerStatefulWidget {
  const PhotoAreaPage({
    required this.image,
    required this.referenceSize,
    super.key,
  });

  final ui.Image image;
  final ReferenceSizeMm referenceSize;

  @override
  ConsumerState<PhotoAreaPage> createState() => _PhotoAreaPageState();
}

class _PhotoAreaPageState extends ConsumerState<PhotoAreaPage> {
  // Dragged point (reference flag + index) and the finger position.
  ({bool isReference, int index})? _dragging;
  Offset? _finger;

  double _scale(Size box) => math.min(
    box.width / widget.image.width,
    box.height / widget.image.height,
  );

  Offset _offset(Size box, double scale) => Offset(
    (box.width - widget.image.width * scale) / 2,
    (box.height - widget.image.height * scale) / 2,
  );

  Point2D _toImage(Offset local, Size box) {
    final s = _scale(box);
    final o = _offset(box, s);
    return Point2D(
      ((local.dx - o.dx) / s).clamp(0, widget.image.width.toDouble()),
      ((local.dy - o.dy) / s).clamp(0, widget.image.height.toDouble()),
    );
  }

  Offset _toScreen(Point2D p, Size box) {
    final s = _scale(box);
    final o = _offset(box, s);
    return Offset(p.x * s + o.dx, p.y * s + o.dy);
  }

  ({bool isReference, int index})? _hit(
    Offset local,
    Size box,
    PhotoAreaState state,
    double radius,
  ) {
    ({bool isReference, int index})? best;
    var bestDist = radius;
    void scan(List<Point2D> pts, {required bool isReference}) {
      for (final (i, p) in pts.indexed) {
        final d = (_toScreen(p, box) - local).distance;
        if (d <= bestDist) {
          bestDist = d;
          best = (isReference: isReference, index: i);
        }
      }
    }

    scan(state.reference, isReference: true);
    scan(state.area, isReference: false);
    return best;
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final state = ref.watch(photoAreaControllerProvider);
    final controller = ref.read(photoAreaControllerProvider.notifier);
    final hitRadius = t.tapTarget / 2;

    final hint = !state.referenceComplete
        ? l10n.photoAreaStepReference(state.reference.length)
        : state.area.length < 3
        ? l10n.photoAreaStepArea(state.area.length)
        : l10n.photoAreaStepAdjust;

    return ErpScreen(
      title: l10n.photoAreaTitle,
      body: Column(
        children: [
          Expanded(
            child: ColoredBox(
              color: t.surfaceMuted,
              child: LayoutBuilder(
                builder: (context, constraints) {
                  final box = constraints.biggest;
                  final s = _scale(box);
                  final o = _offset(box, s);
                  return GestureDetector(
                    behavior: HitTestBehavior.opaque,
                    onTapUp: (d) {
                      if (_hit(d.localPosition, box, state, hitRadius) ==
                          null) {
                        controller.addPoint(_toImage(d.localPosition, box));
                        unawaited(HapticFeedback.selectionClick());
                      }
                    },
                    onPanStart: (d) => setState(() {
                      _dragging = _hit(d.localPosition, box, state, hitRadius);
                      _finger = d.localPosition;
                    }),
                    onPanUpdate: (d) {
                      final drag = _dragging;
                      if (drag == null) return;
                      controller.movePoint(
                        isReference: drag.isReference,
                        index: drag.index,
                        p: _toImage(d.localPosition, box),
                      );
                      setState(() => _finger = d.localPosition);
                    },
                    onPanEnd: (_) => setState(() {
                      _dragging = null;
                      _finger = null;
                    }),
                    child: Stack(
                      children: [
                        Positioned(
                          left: o.dx,
                          top: o.dy,
                          width: widget.image.width * s,
                          height: widget.image.height * s,
                          child: RawImage(
                            image: widget.image,
                            fit: BoxFit.fill,
                          ),
                        ),
                        Positioned.fill(
                          child: CustomPaint(
                            painter: _OverlayPainter(
                              state: state,
                              scale: s,
                              offset: o,
                              tokens: t,
                            ),
                          ),
                        ),
                        if (_dragging != null && _finger != null)
                          _loupe(_finger!, box, t),
                      ],
                    ),
                  );
                },
              ),
            ),
          ),
          _BottomPanel(
            hint: hint,
            state: state,
            size: widget.referenceSize,
            onUndo: controller.undo,
            onClose: controller.close,
            onClear: controller.clear,
          ),
        ],
      ),
    );
  }

  /// Magnifier above the finger (below it near the top edge).
  Widget _loupe(Offset finger, Size box, ErpTokens t) {
    final above = finger.dy - _loupeGap - _loupeSize >= 0;
    final centerY = above
        ? finger.dy - _loupeGap - _loupeSize / 2
        : finger.dy + _loupeGap + _loupeSize / 2;
    final centerX = finger.dx.clamp(_loupeSize / 2, box.width - _loupeSize / 2);
    return Positioned(
      left: centerX - _loupeSize / 2,
      top: centerY - _loupeSize / 2,
      child: IgnorePointer(
        child: Stack(
          alignment: Alignment.center,
          children: [
            RawMagnifier(
              size: const Size.square(_loupeSize),
              magnificationScale: 2.5,
              focalPointOffset: Offset(
                finger.dx - centerX,
                finger.dy - centerY,
              ),
              decoration: MagnifierDecoration(
                shape: RoundedRectangleBorder(
                  borderRadius: t.corner,
                  side: BorderSide(color: t.action, width: t.borderWidth * 3),
                ),
              ),
            ),
            SizedBox(
              width: t.space4,
              height: t.borderWidth,
              child: ColoredBox(color: t.danger),
            ),
            SizedBox(
              width: t.borderWidth,
              height: t.space4,
              child: ColoredBox(color: t.danger),
            ),
          ],
        ),
      ),
    );
  }
}

class _OverlayPainter extends CustomPainter {
  _OverlayPainter({
    required this.state,
    required this.scale,
    required this.offset,
    required this.tokens,
  });

  final PhotoAreaState state;
  final double scale;
  final Offset offset;
  final ErpTokens tokens;

  Offset _s(Point2D p) =>
      Offset(p.x * scale + offset.dx, p.y * scale + offset.dy);

  Path _path(List<Offset> pts, {required bool close}) {
    final path = Path()..moveTo(pts.first.dx, pts.first.dy);
    for (final p in pts.skip(1)) {
      path.lineTo(p.dx, p.dy);
    }
    if (close) path.close();
    return path;
  }

  @override
  void paint(Canvas canvas, Size size) {
    final t = tokens;
    Paint stroke(Color c, double w) => Paint()
      ..color = c
      ..style = PaintingStyle.stroke
      ..strokeWidth = w
      ..strokeJoin = StrokeJoin.miter;

    if (state.reference.length >= 2) {
      final quad = state.referenceComplete
          ? orderQuadClockwise(state.reference)
          : state.reference;
      canvas.drawPath(
        _path(quad.map(_s).toList(), close: state.referenceComplete),
        stroke(t.info, 2),
      );
    }

    if (state.area.length >= 2) {
      final pts = state.area.map(_s).toList();
      final closed = state.closed || state.area.length >= 3;
      final path = _path(pts, close: closed);
      if (state.area.length >= 3) {
        canvas.drawPath(
          path,
          Paint()..color = t.action.withValues(alpha: 0.25),
        );
      }
      canvas.drawPath(path, stroke(t.action, 2.5));
      _dimensionLine(canvas, pts, closed: state.closed, stroke: stroke);
    }

    void handle(Point2D p, Color c) {
      final o = _s(p);
      canvas
        ..drawCircle(o, 10, Paint()..color = t.surface)
        ..drawCircle(o, 7, Paint()..color = c);
    }

    for (final p in state.reference) {
      handle(p, t.info);
    }
    for (final p in state.area) {
      handle(p, t.action);
    }
  }

  /// Measure-accent line with end ticks along the longest drawn edge.
  void _dimensionLine(
    Canvas canvas,
    List<Offset> pts, {
    required bool closed,
    required Paint Function(Color, double) stroke,
  }) {
    var a = pts[0];
    var b = pts[1];
    var longest = 0.0;
    final edges = closed ? pts.length : pts.length - 1;
    for (var i = 0; i < edges; i++) {
      final p = pts[i];
      final q = pts[(i + 1) % pts.length];
      if ((q - p).distance > longest) {
        longest = (q - p).distance;
        a = p;
        b = q;
      }
    }
    if (longest == 0) return;
    final dir = (b - a) / longest;
    final normal = Offset(-dir.dy, dir.dx);
    final shift = normal * 18;
    final paint = stroke(tokens.accentMeasure, 3);
    canvas
      ..drawLine(a + shift, b + shift, paint)
      ..drawLine(a + shift - normal * 7, a + shift + normal * 7, paint)
      ..drawLine(b + shift - normal * 7, b + shift + normal * 7, paint);
  }

  @override
  bool shouldRepaint(_OverlayPainter old) =>
      old.state != state || old.scale != scale || old.offset != offset;
}

class _BottomPanel extends StatelessWidget {
  const _BottomPanel({
    required this.hint,
    required this.state,
    required this.size,
    required this.onUndo,
    required this.onClose,
    required this.onClear,
  });

  final String hint;
  final PhotoAreaState state;
  final ReferenceSizeMm size;
  final VoidCallback onUndo;
  final VoidCallback onClose;
  final VoidCallback onClear;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final locale = Localizations.localeOf(context).toLanguageTag();
    final area = NumberFormat('#,##0.00', locale);
    final mm = NumberFormat('#,##0', locale);
    final problem = state.problem(size);
    final result = state.result(size);
    final hasPoints = state.reference.isNotEmpty;

    return DecoratedBox(
      decoration: BoxDecoration(
        color: t.surface,
        border: Border(
          top: BorderSide(color: t.border, width: t.borderWidth),
        ),
      ),
      child: Padding(
        padding: EdgeInsets.all(t.space3),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Fixed height so the canvas does not jump when the result appears.
            SizedBox(
              height: t.tapTarget * 3,
              child: Align(
                alignment: Alignment.topLeft,
                child: problem != null
                    ? ErpAlert(
                        message: problem == PhotoAreaProblem.selfIntersecting
                            ? l10n.arSelfIntersecting
                            : l10n.photoAreaErrorReference,
                        severity: ErpAlertSeverity.danger,
                      )
                    : result != null
                    ? _ResultBlock(result: result, area: area, mm: mm)
                    : Text(hint, style: text.bodyMedium),
              ),
            ),
            SizedBox(height: t.space3),
            Row(
              children: [
                Expanded(
                  child: ErpButton(
                    label: l10n.arUndo,
                    variant: ErpButtonVariant.outline,
                    onPressed: hasPoints ? onUndo : null,
                  ),
                ),
                SizedBox(width: t.space2),
                Expanded(
                  child: ErpButton(
                    label: l10n.arCloseShape,
                    variant: ErpButtonVariant.outline,
                    onPressed: state.area.length >= 3 && !state.closed
                        ? onClose
                        : null,
                  ),
                ),
                SizedBox(width: t.space2),
                Expanded(
                  child: ErpButton(
                    label: l10n.photoAreaClear,
                    variant: ErpButtonVariant.outline,
                    onPressed: hasPoints ? onClear : null,
                  ),
                ),
              ],
            ),
            if (result != null) ...[
              SizedBox(height: t.space2),
              ErpButton(
                label: l10n.arDone,
                onPressed: () => Navigator.of(context).pop(result),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

class _ResultBlock extends StatelessWidget {
  const _ResultBlock({
    required this.result,
    required this.area,
    required this.mm,
  });

  final PlaneMeasurementResult result;
  final NumberFormat area;
  final NumberFormat mm;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;

    Widget cell(String label, String value) => Expanded(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(label, style: text.bodySmall),
          Text(value, style: text.bodyLarge),
        ],
      ),
    );

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        ErpMeasureDisplay(
          valueText: l10n.arValueSquareMeters(area.format(result.areaSqm)),
        ),
        SizedBox(height: t.space2),
        Row(
          children: [
            cell(
              l10n.photoAreaWidth,
              l10n.photoAreaValueMm(mm.format(result.widthMm)),
            ),
            cell(
              l10n.photoAreaLength,
              l10n.photoAreaValueMm(mm.format(result.lengthMm)),
            ),
            cell(l10n.photoAreaVertices, '${result.vertexCount}'),
          ],
        ),
      ],
    );
  }
}
