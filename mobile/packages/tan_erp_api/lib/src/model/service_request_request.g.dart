// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_request_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceRequestRequest extends ServiceRequestRequest {
  @override
  final String? projectId;
  @override
  final String? title;
  @override
  final String? description;
  @override
  final String? priority;

  factory _$ServiceRequestRequest(
          [void Function(ServiceRequestRequestBuilder)? updates]) =>
      (ServiceRequestRequestBuilder()..update(updates))._build();

  _$ServiceRequestRequest._(
      {this.projectId, this.title, this.description, this.priority})
      : super._();
  @override
  ServiceRequestRequest rebuild(
          void Function(ServiceRequestRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceRequestRequestBuilder toBuilder() =>
      ServiceRequestRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceRequestRequest &&
        projectId == other.projectId &&
        title == other.title &&
        description == other.description &&
        priority == other.priority;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, priority.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ServiceRequestRequest')
          ..add('projectId', projectId)
          ..add('title', title)
          ..add('description', description)
          ..add('priority', priority))
        .toString();
  }
}

class ServiceRequestRequestBuilder
    implements Builder<ServiceRequestRequest, ServiceRequestRequestBuilder> {
  _$ServiceRequestRequest? _$v;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  String? _priority;
  String? get priority => _$this._priority;
  set priority(String? priority) => _$this._priority = priority;

  ServiceRequestRequestBuilder() {
    ServiceRequestRequest._defaults(this);
  }

  ServiceRequestRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _projectId = $v.projectId;
      _title = $v.title;
      _description = $v.description;
      _priority = $v.priority;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServiceRequestRequest other) {
    _$v = other as _$ServiceRequestRequest;
  }

  @override
  void update(void Function(ServiceRequestRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceRequestRequest build() => _build();

  _$ServiceRequestRequest _build() {
    final _$result = _$v ??
        _$ServiceRequestRequest._(
          projectId: projectId,
          title: title,
          description: description,
          priority: priority,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
