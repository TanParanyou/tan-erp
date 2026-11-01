// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bom_draft_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BomDraftRequest extends BomDraftRequest {
  @override
  final double? outputQuantity;
  @override
  final String? note;
  @override
  final BuiltList<BomLineRequest>? lines;
  @override
  final BuiltList<BomRoutingStepRequest>? routingSteps;

  factory _$BomDraftRequest([void Function(BomDraftRequestBuilder)? updates]) =>
      (BomDraftRequestBuilder()..update(updates))._build();

  _$BomDraftRequest._(
      {this.outputQuantity, this.note, this.lines, this.routingSteps})
      : super._();
  @override
  BomDraftRequest rebuild(void Function(BomDraftRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BomDraftRequestBuilder toBuilder() => BomDraftRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BomDraftRequest &&
        outputQuantity == other.outputQuantity &&
        note == other.note &&
        lines == other.lines &&
        routingSteps == other.routingSteps;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, outputQuantity.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jc(_$hash, routingSteps.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BomDraftRequest')
          ..add('outputQuantity', outputQuantity)
          ..add('note', note)
          ..add('lines', lines)
          ..add('routingSteps', routingSteps))
        .toString();
  }
}

class BomDraftRequestBuilder
    implements Builder<BomDraftRequest, BomDraftRequestBuilder> {
  _$BomDraftRequest? _$v;

  double? _outputQuantity;
  double? get outputQuantity => _$this._outputQuantity;
  set outputQuantity(double? outputQuantity) =>
      _$this._outputQuantity = outputQuantity;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ListBuilder<BomLineRequest>? _lines;
  ListBuilder<BomLineRequest> get lines =>
      _$this._lines ??= ListBuilder<BomLineRequest>();
  set lines(ListBuilder<BomLineRequest>? lines) => _$this._lines = lines;

  ListBuilder<BomRoutingStepRequest>? _routingSteps;
  ListBuilder<BomRoutingStepRequest> get routingSteps =>
      _$this._routingSteps ??= ListBuilder<BomRoutingStepRequest>();
  set routingSteps(ListBuilder<BomRoutingStepRequest>? routingSteps) =>
      _$this._routingSteps = routingSteps;

  BomDraftRequestBuilder() {
    BomDraftRequest._defaults(this);
  }

  BomDraftRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _outputQuantity = $v.outputQuantity;
      _note = $v.note;
      _lines = $v.lines?.toBuilder();
      _routingSteps = $v.routingSteps?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BomDraftRequest other) {
    _$v = other as _$BomDraftRequest;
  }

  @override
  void update(void Function(BomDraftRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BomDraftRequest build() => _build();

  _$BomDraftRequest _build() {
    _$BomDraftRequest _$result;
    try {
      _$result = _$v ??
          _$BomDraftRequest._(
            outputQuantity: outputQuantity,
            note: note,
            lines: _lines?.build(),
            routingSteps: _routingSteps?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
        _$failedField = 'routingSteps';
        _routingSteps?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BomDraftRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
