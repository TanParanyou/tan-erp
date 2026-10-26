// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_document_sequence_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateDocumentSequenceRequest extends UpdateDocumentSequenceRequest {
  @override
  final String? prefix;
  @override
  final String formatPattern;
  @override
  final String resetPeriod;
  @override
  final int? padding;
  @override
  final bool? isBranchSpecific;

  factory _$UpdateDocumentSequenceRequest(
          [void Function(UpdateDocumentSequenceRequestBuilder)? updates]) =>
      (UpdateDocumentSequenceRequestBuilder()..update(updates))._build();

  _$UpdateDocumentSequenceRequest._(
      {this.prefix,
      required this.formatPattern,
      required this.resetPeriod,
      this.padding,
      this.isBranchSpecific})
      : super._();
  @override
  UpdateDocumentSequenceRequest rebuild(
          void Function(UpdateDocumentSequenceRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateDocumentSequenceRequestBuilder toBuilder() =>
      UpdateDocumentSequenceRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateDocumentSequenceRequest &&
        prefix == other.prefix &&
        formatPattern == other.formatPattern &&
        resetPeriod == other.resetPeriod &&
        padding == other.padding &&
        isBranchSpecific == other.isBranchSpecific;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, prefix.hashCode);
    _$hash = $jc(_$hash, formatPattern.hashCode);
    _$hash = $jc(_$hash, resetPeriod.hashCode);
    _$hash = $jc(_$hash, padding.hashCode);
    _$hash = $jc(_$hash, isBranchSpecific.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateDocumentSequenceRequest')
          ..add('prefix', prefix)
          ..add('formatPattern', formatPattern)
          ..add('resetPeriod', resetPeriod)
          ..add('padding', padding)
          ..add('isBranchSpecific', isBranchSpecific))
        .toString();
  }
}

class UpdateDocumentSequenceRequestBuilder
    implements
        Builder<UpdateDocumentSequenceRequest,
            UpdateDocumentSequenceRequestBuilder> {
  _$UpdateDocumentSequenceRequest? _$v;

  String? _prefix;
  String? get prefix => _$this._prefix;
  set prefix(String? prefix) => _$this._prefix = prefix;

  String? _formatPattern;
  String? get formatPattern => _$this._formatPattern;
  set formatPattern(String? formatPattern) =>
      _$this._formatPattern = formatPattern;

  String? _resetPeriod;
  String? get resetPeriod => _$this._resetPeriod;
  set resetPeriod(String? resetPeriod) => _$this._resetPeriod = resetPeriod;

  int? _padding;
  int? get padding => _$this._padding;
  set padding(int? padding) => _$this._padding = padding;

  bool? _isBranchSpecific;
  bool? get isBranchSpecific => _$this._isBranchSpecific;
  set isBranchSpecific(bool? isBranchSpecific) =>
      _$this._isBranchSpecific = isBranchSpecific;

  UpdateDocumentSequenceRequestBuilder() {
    UpdateDocumentSequenceRequest._defaults(this);
  }

  UpdateDocumentSequenceRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _prefix = $v.prefix;
      _formatPattern = $v.formatPattern;
      _resetPeriod = $v.resetPeriod;
      _padding = $v.padding;
      _isBranchSpecific = $v.isBranchSpecific;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateDocumentSequenceRequest other) {
    _$v = other as _$UpdateDocumentSequenceRequest;
  }

  @override
  void update(void Function(UpdateDocumentSequenceRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateDocumentSequenceRequest build() => _build();

  _$UpdateDocumentSequenceRequest _build() {
    final _$result = _$v ??
        _$UpdateDocumentSequenceRequest._(
          prefix: prefix,
          formatPattern: BuiltValueNullFieldError.checkNotNull(
              formatPattern, r'UpdateDocumentSequenceRequest', 'formatPattern'),
          resetPeriod: BuiltValueNullFieldError.checkNotNull(
              resetPeriod, r'UpdateDocumentSequenceRequest', 'resetPeriod'),
          padding: padding,
          isBranchSpecific: isBranchSpecific,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
