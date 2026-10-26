// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_revision_diff_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewRevisionDiffResponse
    extends EstimateReviewRevisionDiffResponse {
  @override
  final int? previousRevisionNo;
  @override
  final double? previousGrandTotal;
  @override
  final double? grandTotalDelta;
  @override
  final int? addedWorkItems;
  @override
  final int? removedWorkItems;
  @override
  final int? changedWorkItems;

  factory _$EstimateReviewRevisionDiffResponse(
          [void Function(EstimateReviewRevisionDiffResponseBuilder)?
              updates]) =>
      (EstimateReviewRevisionDiffResponseBuilder()..update(updates))._build();

  _$EstimateReviewRevisionDiffResponse._(
      {this.previousRevisionNo,
      this.previousGrandTotal,
      this.grandTotalDelta,
      this.addedWorkItems,
      this.removedWorkItems,
      this.changedWorkItems})
      : super._();
  @override
  EstimateReviewRevisionDiffResponse rebuild(
          void Function(EstimateReviewRevisionDiffResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewRevisionDiffResponseBuilder toBuilder() =>
      EstimateReviewRevisionDiffResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewRevisionDiffResponse &&
        previousRevisionNo == other.previousRevisionNo &&
        previousGrandTotal == other.previousGrandTotal &&
        grandTotalDelta == other.grandTotalDelta &&
        addedWorkItems == other.addedWorkItems &&
        removedWorkItems == other.removedWorkItems &&
        changedWorkItems == other.changedWorkItems;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, previousRevisionNo.hashCode);
    _$hash = $jc(_$hash, previousGrandTotal.hashCode);
    _$hash = $jc(_$hash, grandTotalDelta.hashCode);
    _$hash = $jc(_$hash, addedWorkItems.hashCode);
    _$hash = $jc(_$hash, removedWorkItems.hashCode);
    _$hash = $jc(_$hash, changedWorkItems.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateReviewRevisionDiffResponse')
          ..add('previousRevisionNo', previousRevisionNo)
          ..add('previousGrandTotal', previousGrandTotal)
          ..add('grandTotalDelta', grandTotalDelta)
          ..add('addedWorkItems', addedWorkItems)
          ..add('removedWorkItems', removedWorkItems)
          ..add('changedWorkItems', changedWorkItems))
        .toString();
  }
}

class EstimateReviewRevisionDiffResponseBuilder
    implements
        Builder<EstimateReviewRevisionDiffResponse,
            EstimateReviewRevisionDiffResponseBuilder> {
  _$EstimateReviewRevisionDiffResponse? _$v;

  int? _previousRevisionNo;
  int? get previousRevisionNo => _$this._previousRevisionNo;
  set previousRevisionNo(int? previousRevisionNo) =>
      _$this._previousRevisionNo = previousRevisionNo;

  double? _previousGrandTotal;
  double? get previousGrandTotal => _$this._previousGrandTotal;
  set previousGrandTotal(double? previousGrandTotal) =>
      _$this._previousGrandTotal = previousGrandTotal;

  double? _grandTotalDelta;
  double? get grandTotalDelta => _$this._grandTotalDelta;
  set grandTotalDelta(double? grandTotalDelta) =>
      _$this._grandTotalDelta = grandTotalDelta;

  int? _addedWorkItems;
  int? get addedWorkItems => _$this._addedWorkItems;
  set addedWorkItems(int? addedWorkItems) =>
      _$this._addedWorkItems = addedWorkItems;

  int? _removedWorkItems;
  int? get removedWorkItems => _$this._removedWorkItems;
  set removedWorkItems(int? removedWorkItems) =>
      _$this._removedWorkItems = removedWorkItems;

  int? _changedWorkItems;
  int? get changedWorkItems => _$this._changedWorkItems;
  set changedWorkItems(int? changedWorkItems) =>
      _$this._changedWorkItems = changedWorkItems;

  EstimateReviewRevisionDiffResponseBuilder() {
    EstimateReviewRevisionDiffResponse._defaults(this);
  }

  EstimateReviewRevisionDiffResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _previousRevisionNo = $v.previousRevisionNo;
      _previousGrandTotal = $v.previousGrandTotal;
      _grandTotalDelta = $v.grandTotalDelta;
      _addedWorkItems = $v.addedWorkItems;
      _removedWorkItems = $v.removedWorkItems;
      _changedWorkItems = $v.changedWorkItems;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewRevisionDiffResponse other) {
    _$v = other as _$EstimateReviewRevisionDiffResponse;
  }

  @override
  void update(
      void Function(EstimateReviewRevisionDiffResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewRevisionDiffResponse build() => _build();

  _$EstimateReviewRevisionDiffResponse _build() {
    final _$result = _$v ??
        _$EstimateReviewRevisionDiffResponse._(
          previousRevisionNo: previousRevisionNo,
          previousGrandTotal: previousGrandTotal,
          grandTotalDelta: grandTotalDelta,
          addedWorkItems: addedWorkItems,
          removedWorkItems: removedWorkItems,
          changedWorkItems: changedWorkItems,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
