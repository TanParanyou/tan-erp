// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'template_decision_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$TemplateDecisionRequest extends TemplateDecisionRequest {
  @override
  final String? decision;
  @override
  final String? note;

  factory _$TemplateDecisionRequest(
          [void Function(TemplateDecisionRequestBuilder)? updates]) =>
      (TemplateDecisionRequestBuilder()..update(updates))._build();

  _$TemplateDecisionRequest._({this.decision, this.note}) : super._();
  @override
  TemplateDecisionRequest rebuild(
          void Function(TemplateDecisionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  TemplateDecisionRequestBuilder toBuilder() =>
      TemplateDecisionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is TemplateDecisionRequest &&
        decision == other.decision &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, decision.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'TemplateDecisionRequest')
          ..add('decision', decision)
          ..add('note', note))
        .toString();
  }
}

class TemplateDecisionRequestBuilder
    implements
        Builder<TemplateDecisionRequest, TemplateDecisionRequestBuilder> {
  _$TemplateDecisionRequest? _$v;

  String? _decision;
  String? get decision => _$this._decision;
  set decision(String? decision) => _$this._decision = decision;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  TemplateDecisionRequestBuilder() {
    TemplateDecisionRequest._defaults(this);
  }

  TemplateDecisionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _decision = $v.decision;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(TemplateDecisionRequest other) {
    _$v = other as _$TemplateDecisionRequest;
  }

  @override
  void update(void Function(TemplateDecisionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  TemplateDecisionRequest build() => _build();

  _$TemplateDecisionRequest _build() {
    final _$result = _$v ??
        _$TemplateDecisionRequest._(
          decision: decision,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
