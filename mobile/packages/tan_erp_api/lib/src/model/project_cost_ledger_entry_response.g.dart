// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_cost_ledger_entry_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectCostLedgerEntryResponse extends ProjectCostLedgerEntryResponse {
  @override
  final String? id;
  @override
  final String? wbsNodeId;
  @override
  final String? wbsCode;
  @override
  final String? category;
  @override
  final String? entryType;
  @override
  final String? sourceType;
  @override
  final String? sourceId;
  @override
  final String? sourceNumber;
  @override
  final String? description;
  @override
  final double? amount;
  @override
  final bool? isReversal;
  @override
  final ProjectPersonResponse? actor;
  @override
  final DateTime? occurredAtUtc;

  factory _$ProjectCostLedgerEntryResponse(
          [void Function(ProjectCostLedgerEntryResponseBuilder)? updates]) =>
      (ProjectCostLedgerEntryResponseBuilder()..update(updates))._build();

  _$ProjectCostLedgerEntryResponse._(
      {this.id,
      this.wbsNodeId,
      this.wbsCode,
      this.category,
      this.entryType,
      this.sourceType,
      this.sourceId,
      this.sourceNumber,
      this.description,
      this.amount,
      this.isReversal,
      this.actor,
      this.occurredAtUtc})
      : super._();
  @override
  ProjectCostLedgerEntryResponse rebuild(
          void Function(ProjectCostLedgerEntryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectCostLedgerEntryResponseBuilder toBuilder() =>
      ProjectCostLedgerEntryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectCostLedgerEntryResponse &&
        id == other.id &&
        wbsNodeId == other.wbsNodeId &&
        wbsCode == other.wbsCode &&
        category == other.category &&
        entryType == other.entryType &&
        sourceType == other.sourceType &&
        sourceId == other.sourceId &&
        sourceNumber == other.sourceNumber &&
        description == other.description &&
        amount == other.amount &&
        isReversal == other.isReversal &&
        actor == other.actor &&
        occurredAtUtc == other.occurredAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, wbsNodeId.hashCode);
    _$hash = $jc(_$hash, wbsCode.hashCode);
    _$hash = $jc(_$hash, category.hashCode);
    _$hash = $jc(_$hash, entryType.hashCode);
    _$hash = $jc(_$hash, sourceType.hashCode);
    _$hash = $jc(_$hash, sourceId.hashCode);
    _$hash = $jc(_$hash, sourceNumber.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, isReversal.hashCode);
    _$hash = $jc(_$hash, actor.hashCode);
    _$hash = $jc(_$hash, occurredAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectCostLedgerEntryResponse')
          ..add('id', id)
          ..add('wbsNodeId', wbsNodeId)
          ..add('wbsCode', wbsCode)
          ..add('category', category)
          ..add('entryType', entryType)
          ..add('sourceType', sourceType)
          ..add('sourceId', sourceId)
          ..add('sourceNumber', sourceNumber)
          ..add('description', description)
          ..add('amount', amount)
          ..add('isReversal', isReversal)
          ..add('actor', actor)
          ..add('occurredAtUtc', occurredAtUtc))
        .toString();
  }
}

class ProjectCostLedgerEntryResponseBuilder
    implements
        Builder<ProjectCostLedgerEntryResponse,
            ProjectCostLedgerEntryResponseBuilder> {
  _$ProjectCostLedgerEntryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _wbsNodeId;
  String? get wbsNodeId => _$this._wbsNodeId;
  set wbsNodeId(String? wbsNodeId) => _$this._wbsNodeId = wbsNodeId;

  String? _wbsCode;
  String? get wbsCode => _$this._wbsCode;
  set wbsCode(String? wbsCode) => _$this._wbsCode = wbsCode;

  String? _category;
  String? get category => _$this._category;
  set category(String? category) => _$this._category = category;

  String? _entryType;
  String? get entryType => _$this._entryType;
  set entryType(String? entryType) => _$this._entryType = entryType;

  String? _sourceType;
  String? get sourceType => _$this._sourceType;
  set sourceType(String? sourceType) => _$this._sourceType = sourceType;

  String? _sourceId;
  String? get sourceId => _$this._sourceId;
  set sourceId(String? sourceId) => _$this._sourceId = sourceId;

  String? _sourceNumber;
  String? get sourceNumber => _$this._sourceNumber;
  set sourceNumber(String? sourceNumber) => _$this._sourceNumber = sourceNumber;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  bool? _isReversal;
  bool? get isReversal => _$this._isReversal;
  set isReversal(bool? isReversal) => _$this._isReversal = isReversal;

  ProjectPersonResponseBuilder? _actor;
  ProjectPersonResponseBuilder get actor =>
      _$this._actor ??= ProjectPersonResponseBuilder();
  set actor(ProjectPersonResponseBuilder? actor) => _$this._actor = actor;

  DateTime? _occurredAtUtc;
  DateTime? get occurredAtUtc => _$this._occurredAtUtc;
  set occurredAtUtc(DateTime? occurredAtUtc) =>
      _$this._occurredAtUtc = occurredAtUtc;

  ProjectCostLedgerEntryResponseBuilder() {
    ProjectCostLedgerEntryResponse._defaults(this);
  }

  ProjectCostLedgerEntryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _wbsNodeId = $v.wbsNodeId;
      _wbsCode = $v.wbsCode;
      _category = $v.category;
      _entryType = $v.entryType;
      _sourceType = $v.sourceType;
      _sourceId = $v.sourceId;
      _sourceNumber = $v.sourceNumber;
      _description = $v.description;
      _amount = $v.amount;
      _isReversal = $v.isReversal;
      _actor = $v.actor?.toBuilder();
      _occurredAtUtc = $v.occurredAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectCostLedgerEntryResponse other) {
    _$v = other as _$ProjectCostLedgerEntryResponse;
  }

  @override
  void update(void Function(ProjectCostLedgerEntryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectCostLedgerEntryResponse build() => _build();

  _$ProjectCostLedgerEntryResponse _build() {
    _$ProjectCostLedgerEntryResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectCostLedgerEntryResponse._(
            id: id,
            wbsNodeId: wbsNodeId,
            wbsCode: wbsCode,
            category: category,
            entryType: entryType,
            sourceType: sourceType,
            sourceId: sourceId,
            sourceNumber: sourceNumber,
            description: description,
            amount: amount,
            isReversal: isReversal,
            actor: _actor?.build(),
            occurredAtUtc: occurredAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'actor';
        _actor?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectCostLedgerEntryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
