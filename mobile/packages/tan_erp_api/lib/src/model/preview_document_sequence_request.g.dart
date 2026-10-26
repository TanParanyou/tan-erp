// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'preview_document_sequence_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PreviewDocumentSequenceRequest extends PreviewDocumentSequenceRequest {
  @override
  final String? prefix;
  @override
  final String? formatPattern;
  @override
  final String? branchCode;
  @override
  final int? padding;
  @override
  final int? sampleSequence;

  factory _$PreviewDocumentSequenceRequest(
          [void Function(PreviewDocumentSequenceRequestBuilder)? updates]) =>
      (PreviewDocumentSequenceRequestBuilder()..update(updates))._build();

  _$PreviewDocumentSequenceRequest._(
      {this.prefix,
      this.formatPattern,
      this.branchCode,
      this.padding,
      this.sampleSequence})
      : super._();
  @override
  PreviewDocumentSequenceRequest rebuild(
          void Function(PreviewDocumentSequenceRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PreviewDocumentSequenceRequestBuilder toBuilder() =>
      PreviewDocumentSequenceRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PreviewDocumentSequenceRequest &&
        prefix == other.prefix &&
        formatPattern == other.formatPattern &&
        branchCode == other.branchCode &&
        padding == other.padding &&
        sampleSequence == other.sampleSequence;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, prefix.hashCode);
    _$hash = $jc(_$hash, formatPattern.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, padding.hashCode);
    _$hash = $jc(_$hash, sampleSequence.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PreviewDocumentSequenceRequest')
          ..add('prefix', prefix)
          ..add('formatPattern', formatPattern)
          ..add('branchCode', branchCode)
          ..add('padding', padding)
          ..add('sampleSequence', sampleSequence))
        .toString();
  }
}

class PreviewDocumentSequenceRequestBuilder
    implements
        Builder<PreviewDocumentSequenceRequest,
            PreviewDocumentSequenceRequestBuilder> {
  _$PreviewDocumentSequenceRequest? _$v;

  String? _prefix;
  String? get prefix => _$this._prefix;
  set prefix(String? prefix) => _$this._prefix = prefix;

  String? _formatPattern;
  String? get formatPattern => _$this._formatPattern;
  set formatPattern(String? formatPattern) =>
      _$this._formatPattern = formatPattern;

  String? _branchCode;
  String? get branchCode => _$this._branchCode;
  set branchCode(String? branchCode) => _$this._branchCode = branchCode;

  int? _padding;
  int? get padding => _$this._padding;
  set padding(int? padding) => _$this._padding = padding;

  int? _sampleSequence;
  int? get sampleSequence => _$this._sampleSequence;
  set sampleSequence(int? sampleSequence) =>
      _$this._sampleSequence = sampleSequence;

  PreviewDocumentSequenceRequestBuilder() {
    PreviewDocumentSequenceRequest._defaults(this);
  }

  PreviewDocumentSequenceRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _prefix = $v.prefix;
      _formatPattern = $v.formatPattern;
      _branchCode = $v.branchCode;
      _padding = $v.padding;
      _sampleSequence = $v.sampleSequence;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PreviewDocumentSequenceRequest other) {
    _$v = other as _$PreviewDocumentSequenceRequest;
  }

  @override
  void update(void Function(PreviewDocumentSequenceRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PreviewDocumentSequenceRequest build() => _build();

  _$PreviewDocumentSequenceRequest _build() {
    final _$result = _$v ??
        _$PreviewDocumentSequenceRequest._(
          prefix: prefix,
          formatPattern: formatPattern,
          branchCode: branchCode,
          padding: padding,
          sampleSequence: sampleSequence,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
