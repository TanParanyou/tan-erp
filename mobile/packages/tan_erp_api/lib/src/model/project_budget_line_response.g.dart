// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_budget_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectBudgetLineResponse extends ProjectBudgetLineResponse {
  @override
  final String? id;
  @override
  final String? category;
  @override
  final String? description;
  @override
  final double? amount;
  @override
  final int? sortOrder;
  @override
  final String? wbsNodeId;
  @override
  final String? wbsCode;
  @override
  final String? wbsName;

  factory _$ProjectBudgetLineResponse(
          [void Function(ProjectBudgetLineResponseBuilder)? updates]) =>
      (ProjectBudgetLineResponseBuilder()..update(updates))._build();

  _$ProjectBudgetLineResponse._(
      {this.id,
      this.category,
      this.description,
      this.amount,
      this.sortOrder,
      this.wbsNodeId,
      this.wbsCode,
      this.wbsName})
      : super._();
  @override
  ProjectBudgetLineResponse rebuild(
          void Function(ProjectBudgetLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectBudgetLineResponseBuilder toBuilder() =>
      ProjectBudgetLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectBudgetLineResponse &&
        id == other.id &&
        category == other.category &&
        description == other.description &&
        amount == other.amount &&
        sortOrder == other.sortOrder &&
        wbsNodeId == other.wbsNodeId &&
        wbsCode == other.wbsCode &&
        wbsName == other.wbsName;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, category.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, wbsNodeId.hashCode);
    _$hash = $jc(_$hash, wbsCode.hashCode);
    _$hash = $jc(_$hash, wbsName.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectBudgetLineResponse')
          ..add('id', id)
          ..add('category', category)
          ..add('description', description)
          ..add('amount', amount)
          ..add('sortOrder', sortOrder)
          ..add('wbsNodeId', wbsNodeId)
          ..add('wbsCode', wbsCode)
          ..add('wbsName', wbsName))
        .toString();
  }
}

class ProjectBudgetLineResponseBuilder
    implements
        Builder<ProjectBudgetLineResponse, ProjectBudgetLineResponseBuilder> {
  _$ProjectBudgetLineResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _category;
  String? get category => _$this._category;
  set category(String? category) => _$this._category = category;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  String? _wbsNodeId;
  String? get wbsNodeId => _$this._wbsNodeId;
  set wbsNodeId(String? wbsNodeId) => _$this._wbsNodeId = wbsNodeId;

  String? _wbsCode;
  String? get wbsCode => _$this._wbsCode;
  set wbsCode(String? wbsCode) => _$this._wbsCode = wbsCode;

  String? _wbsName;
  String? get wbsName => _$this._wbsName;
  set wbsName(String? wbsName) => _$this._wbsName = wbsName;

  ProjectBudgetLineResponseBuilder() {
    ProjectBudgetLineResponse._defaults(this);
  }

  ProjectBudgetLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _category = $v.category;
      _description = $v.description;
      _amount = $v.amount;
      _sortOrder = $v.sortOrder;
      _wbsNodeId = $v.wbsNodeId;
      _wbsCode = $v.wbsCode;
      _wbsName = $v.wbsName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectBudgetLineResponse other) {
    _$v = other as _$ProjectBudgetLineResponse;
  }

  @override
  void update(void Function(ProjectBudgetLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectBudgetLineResponse build() => _build();

  _$ProjectBudgetLineResponse _build() {
    final _$result = _$v ??
        _$ProjectBudgetLineResponse._(
          id: id,
          category: category,
          description: description,
          amount: amount,
          sortOrder: sortOrder,
          wbsNodeId: wbsNodeId,
          wbsCode: wbsCode,
          wbsName: wbsName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
