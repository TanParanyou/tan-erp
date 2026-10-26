//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/warranty_ref_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/service_request_event_response.dart';
import 'package:tan_erp_api/src/model/service_person_response.dart';
import 'package:tan_erp_api/src/model/service_project_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'service_request_response.g.dart';

/// ServiceRequestResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [number] 
/// * [title] 
/// * [description] 
/// * [priority] 
/// * [status] 
/// * [inWarranty] 
/// * [warranty] 
/// * [project] 
/// * [scheduledDate] 
/// * [resolutionNote] 
/// * [reopenCount] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [rowVersion] 
/// * [events] 
@BuiltValue()
abstract class ServiceRequestResponse implements Built<ServiceRequestResponse, ServiceRequestResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'title')
  String? get title;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'priority')
  String? get priority;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'inWarranty')
  bool? get inWarranty;

  @BuiltValueField(wireName: r'warranty')
  WarrantyRefResponse? get warranty;

  @BuiltValueField(wireName: r'project')
  ServiceProjectRefResponse? get project;

  @BuiltValueField(wireName: r'scheduledDate')
  Date? get scheduledDate;

  @BuiltValueField(wireName: r'resolutionNote')
  String? get resolutionNote;

  @BuiltValueField(wireName: r'reopenCount')
  int? get reopenCount;

  @BuiltValueField(wireName: r'createdBy')
  ServicePersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'events')
  BuiltList<ServiceRequestEventResponse>? get events;

  ServiceRequestResponse._();

  factory ServiceRequestResponse([void updates(ServiceRequestResponseBuilder b)]) = _$ServiceRequestResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ServiceRequestResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ServiceRequestResponse> get serializer => _$ServiceRequestResponseSerializer();
}

class _$ServiceRequestResponseSerializer implements PrimitiveSerializer<ServiceRequestResponse> {
  @override
  final Iterable<Type> types = const [ServiceRequestResponse, _$ServiceRequestResponse];

  @override
  final String wireName = r'ServiceRequestResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ServiceRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.title != null) {
      yield r'title';
      yield serializers.serialize(
        object.title,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.priority != null) {
      yield r'priority';
      yield serializers.serialize(
        object.priority,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.inWarranty != null) {
      yield r'inWarranty';
      yield serializers.serialize(
        object.inWarranty,
        specifiedType: const FullType(bool),
      );
    }
    if (object.warranty != null) {
      yield r'warranty';
      yield serializers.serialize(
        object.warranty,
        specifiedType: const FullType(WarrantyRefResponse),
      );
    }
    if (object.project != null) {
      yield r'project';
      yield serializers.serialize(
        object.project,
        specifiedType: const FullType(ServiceProjectRefResponse),
      );
    }
    if (object.scheduledDate != null) {
      yield r'scheduledDate';
      yield serializers.serialize(
        object.scheduledDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.resolutionNote != null) {
      yield r'resolutionNote';
      yield serializers.serialize(
        object.resolutionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reopenCount != null) {
      yield r'reopenCount';
      yield serializers.serialize(
        object.reopenCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(ServicePersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.events != null) {
      yield r'events';
      yield serializers.serialize(
        object.events,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ServiceRequestEventResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ServiceRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ServiceRequestResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'title':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.title = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'priority':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.priority = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'inWarranty':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.inWarranty = valueDes;
          break;
        case r'warranty':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(WarrantyRefResponse),
          ) as WarrantyRefResponse?;
          if (valueDes == null) continue;
          result.warranty.replace(valueDes);
          break;
        case r'project':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServiceProjectRefResponse),
          ) as ServiceProjectRefResponse?;
          if (valueDes == null) continue;
          result.project.replace(valueDes);
          break;
        case r'scheduledDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.scheduledDate = valueDes;
          break;
        case r'resolutionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.resolutionNote = valueDes;
          break;
        case r'reopenCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.reopenCount = valueDes;
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePersonResponse),
          ) as ServicePersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'events':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ServiceRequestEventResponse)]),
          ) as BuiltList<ServiceRequestEventResponse>?;
          if (valueDes == null) continue;
          result.events.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ServiceRequestResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ServiceRequestResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


