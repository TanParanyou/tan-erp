// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calculation_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CalculationResponse extends CalculationResponse {
  @override
  final int? version;
  @override
  final String? templateCode;
  @override
  final int? templateVersion;
  @override
  final double? netAmount;
  @override
  final double? taxAmount;
  @override
  final double? displayedLower;
  @override
  final double? displayedUpper;
  @override
  final Date? validUntil;
  @override
  final String? shareDecision;
  @override
  final BuiltList<String>? reasonCodes;
  @override
  final String? inputHash;
  @override
  final BuiltList<CalculationLineResponse>? lines;
  @override
  final QePersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? branchOverrideId;

  factory _$CalculationResponse(
          [void Function(CalculationResponseBuilder)? updates]) =>
      (CalculationResponseBuilder()..update(updates))._build();

  _$CalculationResponse._(
      {this.version,
      this.templateCode,
      this.templateVersion,
      this.netAmount,
      this.taxAmount,
      this.displayedLower,
      this.displayedUpper,
      this.validUntil,
      this.shareDecision,
      this.reasonCodes,
      this.inputHash,
      this.lines,
      this.createdBy,
      this.createdAtUtc,
      this.branchOverrideId})
      : super._();
  @override
  CalculationResponse rebuild(
          void Function(CalculationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CalculationResponseBuilder toBuilder() =>
      CalculationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CalculationResponse &&
        version == other.version &&
        templateCode == other.templateCode &&
        templateVersion == other.templateVersion &&
        netAmount == other.netAmount &&
        taxAmount == other.taxAmount &&
        displayedLower == other.displayedLower &&
        displayedUpper == other.displayedUpper &&
        validUntil == other.validUntil &&
        shareDecision == other.shareDecision &&
        reasonCodes == other.reasonCodes &&
        inputHash == other.inputHash &&
        lines == other.lines &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        branchOverrideId == other.branchOverrideId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, version.hashCode);
    _$hash = $jc(_$hash, templateCode.hashCode);
    _$hash = $jc(_$hash, templateVersion.hashCode);
    _$hash = $jc(_$hash, netAmount.hashCode);
    _$hash = $jc(_$hash, taxAmount.hashCode);
    _$hash = $jc(_$hash, displayedLower.hashCode);
    _$hash = $jc(_$hash, displayedUpper.hashCode);
    _$hash = $jc(_$hash, validUntil.hashCode);
    _$hash = $jc(_$hash, shareDecision.hashCode);
    _$hash = $jc(_$hash, reasonCodes.hashCode);
    _$hash = $jc(_$hash, inputHash.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, branchOverrideId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CalculationResponse')
          ..add('version', version)
          ..add('templateCode', templateCode)
          ..add('templateVersion', templateVersion)
          ..add('netAmount', netAmount)
          ..add('taxAmount', taxAmount)
          ..add('displayedLower', displayedLower)
          ..add('displayedUpper', displayedUpper)
          ..add('validUntil', validUntil)
          ..add('shareDecision', shareDecision)
          ..add('reasonCodes', reasonCodes)
          ..add('inputHash', inputHash)
          ..add('lines', lines)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('branchOverrideId', branchOverrideId))
        .toString();
  }
}

class CalculationResponseBuilder
    implements Builder<CalculationResponse, CalculationResponseBuilder> {
  _$CalculationResponse? _$v;

  int? _version;
  int? get version => _$this._version;
  set version(int? version) => _$this._version = version;

  String? _templateCode;
  String? get templateCode => _$this._templateCode;
  set templateCode(String? templateCode) => _$this._templateCode = templateCode;

  int? _templateVersion;
  int? get templateVersion => _$this._templateVersion;
  set templateVersion(int? templateVersion) =>
      _$this._templateVersion = templateVersion;

  double? _netAmount;
  double? get netAmount => _$this._netAmount;
  set netAmount(double? netAmount) => _$this._netAmount = netAmount;

  double? _taxAmount;
  double? get taxAmount => _$this._taxAmount;
  set taxAmount(double? taxAmount) => _$this._taxAmount = taxAmount;

  double? _displayedLower;
  double? get displayedLower => _$this._displayedLower;
  set displayedLower(double? displayedLower) =>
      _$this._displayedLower = displayedLower;

  double? _displayedUpper;
  double? get displayedUpper => _$this._displayedUpper;
  set displayedUpper(double? displayedUpper) =>
      _$this._displayedUpper = displayedUpper;

  Date? _validUntil;
  Date? get validUntil => _$this._validUntil;
  set validUntil(Date? validUntil) => _$this._validUntil = validUntil;

  String? _shareDecision;
  String? get shareDecision => _$this._shareDecision;
  set shareDecision(String? shareDecision) =>
      _$this._shareDecision = shareDecision;

  ListBuilder<String>? _reasonCodes;
  ListBuilder<String> get reasonCodes =>
      _$this._reasonCodes ??= ListBuilder<String>();
  set reasonCodes(ListBuilder<String>? reasonCodes) =>
      _$this._reasonCodes = reasonCodes;

  String? _inputHash;
  String? get inputHash => _$this._inputHash;
  set inputHash(String? inputHash) => _$this._inputHash = inputHash;

  ListBuilder<CalculationLineResponse>? _lines;
  ListBuilder<CalculationLineResponse> get lines =>
      _$this._lines ??= ListBuilder<CalculationLineResponse>();
  set lines(ListBuilder<CalculationLineResponse>? lines) =>
      _$this._lines = lines;

  QePersonResponseBuilder? _createdBy;
  QePersonResponseBuilder get createdBy =>
      _$this._createdBy ??= QePersonResponseBuilder();
  set createdBy(QePersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  String? _branchOverrideId;
  String? get branchOverrideId => _$this._branchOverrideId;
  set branchOverrideId(String? branchOverrideId) =>
      _$this._branchOverrideId = branchOverrideId;

  CalculationResponseBuilder() {
    CalculationResponse._defaults(this);
  }

  CalculationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _version = $v.version;
      _templateCode = $v.templateCode;
      _templateVersion = $v.templateVersion;
      _netAmount = $v.netAmount;
      _taxAmount = $v.taxAmount;
      _displayedLower = $v.displayedLower;
      _displayedUpper = $v.displayedUpper;
      _validUntil = $v.validUntil;
      _shareDecision = $v.shareDecision;
      _reasonCodes = $v.reasonCodes?.toBuilder();
      _inputHash = $v.inputHash;
      _lines = $v.lines?.toBuilder();
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _branchOverrideId = $v.branchOverrideId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CalculationResponse other) {
    _$v = other as _$CalculationResponse;
  }

  @override
  void update(void Function(CalculationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CalculationResponse build() => _build();

  _$CalculationResponse _build() {
    _$CalculationResponse _$result;
    try {
      _$result = _$v ??
          _$CalculationResponse._(
            version: version,
            templateCode: templateCode,
            templateVersion: templateVersion,
            netAmount: netAmount,
            taxAmount: taxAmount,
            displayedLower: displayedLower,
            displayedUpper: displayedUpper,
            validUntil: validUntil,
            shareDecision: shareDecision,
            reasonCodes: _reasonCodes?.build(),
            inputHash: inputHash,
            lines: _lines?.build(),
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            branchOverrideId: branchOverrideId,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'reasonCodes';
        _reasonCodes?.build();

        _$failedField = 'lines';
        _lines?.build();
        _$failedField = 'createdBy';
        _createdBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CalculationResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
