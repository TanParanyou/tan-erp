// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'dispatch_result_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DispatchResultResponse extends DispatchResultResponse {
  @override
  final int? processed;
  @override
  final int? sent;
  @override
  final int? failed;
  @override
  final int? dead;

  factory _$DispatchResultResponse(
          [void Function(DispatchResultResponseBuilder)? updates]) =>
      (DispatchResultResponseBuilder()..update(updates))._build();

  _$DispatchResultResponse._(
      {this.processed, this.sent, this.failed, this.dead})
      : super._();
  @override
  DispatchResultResponse rebuild(
          void Function(DispatchResultResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DispatchResultResponseBuilder toBuilder() =>
      DispatchResultResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DispatchResultResponse &&
        processed == other.processed &&
        sent == other.sent &&
        failed == other.failed &&
        dead == other.dead;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, processed.hashCode);
    _$hash = $jc(_$hash, sent.hashCode);
    _$hash = $jc(_$hash, failed.hashCode);
    _$hash = $jc(_$hash, dead.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DispatchResultResponse')
          ..add('processed', processed)
          ..add('sent', sent)
          ..add('failed', failed)
          ..add('dead', dead))
        .toString();
  }
}

class DispatchResultResponseBuilder
    implements Builder<DispatchResultResponse, DispatchResultResponseBuilder> {
  _$DispatchResultResponse? _$v;

  int? _processed;
  int? get processed => _$this._processed;
  set processed(int? processed) => _$this._processed = processed;

  int? _sent;
  int? get sent => _$this._sent;
  set sent(int? sent) => _$this._sent = sent;

  int? _failed;
  int? get failed => _$this._failed;
  set failed(int? failed) => _$this._failed = failed;

  int? _dead;
  int? get dead => _$this._dead;
  set dead(int? dead) => _$this._dead = dead;

  DispatchResultResponseBuilder() {
    DispatchResultResponse._defaults(this);
  }

  DispatchResultResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _processed = $v.processed;
      _sent = $v.sent;
      _failed = $v.failed;
      _dead = $v.dead;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DispatchResultResponse other) {
    _$v = other as _$DispatchResultResponse;
  }

  @override
  void update(void Function(DispatchResultResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DispatchResultResponse build() => _build();

  _$DispatchResultResponse _build() {
    final _$result = _$v ??
        _$DispatchResultResponse._(
          processed: processed,
          sent: sent,
          failed: failed,
          dead: dead,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
