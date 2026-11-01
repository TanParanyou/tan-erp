//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'service_sla_response.g.dart';

/// ServiceSlaResponse
///
/// Properties:
/// * [responseHours] 
/// * [resolveHours] 
/// * [elapsedHours] 
/// * [pausedSeconds] 
/// * [status] 
@BuiltValue()
abstract class ServiceSlaResponse implements Built<ServiceSlaResponse, ServiceSlaResponseBuilder> {
  @BuiltValueField(wireName: r'responseHours')
  int? get responseHours;

  @BuiltValueField(wireName: r'resolveHours')
  int? get resolveHours;

  @BuiltValueField(wireName: r'elapsedHours')
  double? get elapsedHours;

  @BuiltValueField(wireName: r'pausedSeconds')
  int? get pausedSeconds;

  @BuiltValueField(wireName: r'status')
  String? get status;

  ServiceSlaResponse._();

  factory ServiceSlaResponse([void updates(ServiceSlaResponseBuilder b)]) = _$ServiceSlaResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ServiceSlaResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ServiceSlaResponse> get serializer => _$ServiceSlaResponseSerializer();
}

class _$ServiceSlaResponseSerializer implements PrimitiveSerializer<ServiceSlaResponse> {
  @override
  final Iterable<Type> types = const [ServiceSlaResponse, _$ServiceSlaResponse];

  @override
  final String wireName = r'ServiceSlaResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ServiceSlaResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.responseHours != null) {
      yield r'responseHours';
      yield serializers.serialize(
        object.responseHours,
        specifiedType: const FullType(int),
      );
    }
    if (object.resolveHours != null) {
      yield r'resolveHours';
      yield serializers.serialize(
        object.resolveHours,
        specifiedType: const FullType(int),
      );
    }
    if (object.elapsedHours != null) {
      yield r'elapsedHours';
      yield serializers.serialize(
        object.elapsedHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.pausedSeconds != null) {
      yield r'pausedSeconds';
      yield serializers.serialize(
        object.pausedSeconds,
        specifiedType: const FullType(int),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ServiceSlaResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ServiceSlaResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'responseHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.responseHours = valueDes;
          break;
        case r'resolveHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.resolveHours = valueDes;
          break;
        case r'elapsedHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.elapsedHours = valueDes;
          break;
        case r'pausedSeconds':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.pausedSeconds = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ServiceSlaResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ServiceSlaResponseBuilder();
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


