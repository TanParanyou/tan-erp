//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'service_request_list_item_response.g.dart';

/// ServiceRequestListItemResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [title] 
/// * [priority] 
/// * [status] 
/// * [inWarranty] 
/// * [projectCode] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class ServiceRequestListItemResponse implements Built<ServiceRequestListItemResponse, ServiceRequestListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'title')
  String? get title;

  @BuiltValueField(wireName: r'priority')
  String? get priority;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'inWarranty')
  bool? get inWarranty;

  @BuiltValueField(wireName: r'projectCode')
  String? get projectCode;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  ServiceRequestListItemResponse._();

  factory ServiceRequestListItemResponse([void updates(ServiceRequestListItemResponseBuilder b)]) = _$ServiceRequestListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ServiceRequestListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ServiceRequestListItemResponse> get serializer => _$ServiceRequestListItemResponseSerializer();
}

class _$ServiceRequestListItemResponseSerializer implements PrimitiveSerializer<ServiceRequestListItemResponse> {
  @override
  final Iterable<Type> types = const [ServiceRequestListItemResponse, _$ServiceRequestListItemResponse];

  @override
  final String wireName = r'ServiceRequestListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ServiceRequestListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
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
    if (object.projectCode != null) {
      yield r'projectCode';
      yield serializers.serialize(
        object.projectCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ServiceRequestListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ServiceRequestListItemResponseBuilder result,
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
        case r'projectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectCode = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ServiceRequestListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ServiceRequestListItemResponseBuilder();
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


