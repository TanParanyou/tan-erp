import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/geometry/plane_measurement.dart';

enum PhotoAreaProblem { referenceInvalid, selfIntersecting }

@immutable
class PhotoAreaState {
  const PhotoAreaState({
    this.reference = const [],
    this.area = const [],
    this.closed = false,
  });

  /// Points are in image pixels, never screen pixels.
  final List<Point2D> reference;
  final List<Point2D> area;
  final bool closed;

  bool get referenceComplete => reference.length == 4;
  bool get canMeasure => referenceComplete && area.length >= 3;

  PhotoAreaProblem? problem(ReferenceSizeMm size) {
    if (!canMeasure) return null;
    if (!isSimplePolygon(area)) return PhotoAreaProblem.selfIntersecting;
    return _measure(size) == null ? PhotoAreaProblem.referenceInvalid : null;
  }

  PlaneMeasurementResult? result(ReferenceSizeMm size) =>
      canMeasure && isSimplePolygon(area) ? _measure(size) : null;

  PlaneMeasurementResult? _measure(ReferenceSizeMm size) => measurePlanePolygon(
    referenceQuadPx: reference,
    referenceSizeMm: size,
    polygonPx: area,
  );
}

/// Points of the photo-area page. Nothing is persisted (Device Lab only).
class PhotoAreaController extends Notifier<PhotoAreaState> {
  @override
  PhotoAreaState build() => const PhotoAreaState();

  /// Adds the next reference corner, then area vertices.
  void addPoint(Point2D p) {
    if (state.closed) return;
    state = state.referenceComplete
        ? PhotoAreaState(reference: state.reference, area: [...state.area, p])
        : PhotoAreaState(reference: [...state.reference, p]);
  }

  void movePoint({
    required bool isReference,
    required int index,
    required Point2D p,
  }) {
    final list = [...(isReference ? state.reference : state.area)];
    list[index] = p;
    state = PhotoAreaState(
      reference: isReference ? list : state.reference,
      area: isReference ? state.area : list,
      closed: state.closed,
    );
  }

  void undo() {
    if (state.closed) {
      state = PhotoAreaState(reference: state.reference, area: state.area);
    } else if (state.area.isNotEmpty) {
      state = PhotoAreaState(
        reference: state.reference,
        area: state.area.sublist(0, state.area.length - 1),
      );
    } else if (state.reference.isNotEmpty) {
      state = PhotoAreaState(
        reference: state.reference.sublist(0, state.reference.length - 1),
      );
    }
  }

  void close() {
    if (state.area.length >= 3) {
      state = PhotoAreaState(
        reference: state.reference,
        area: state.area,
        closed: true,
      );
    }
  }

  void clear() => state = const PhotoAreaState();
}

final NotifierProvider<PhotoAreaController, PhotoAreaState>
photoAreaControllerProvider = NotifierProvider.autoDispose(
  PhotoAreaController.new,
);
