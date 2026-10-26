//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'release_reservation_request.g.dart';

/// ReleaseReservationRequest
///
/// Properties:
/// * [expectedVersion] 
@BuiltValue()
abstract class ReleaseReservationRequest implements Built<ReleaseReservationRequest, ReleaseReservationRequestBuilder> {
  @BuiltValueField(wireName: r'expectedVersion')
  String? get expectedVersion;

  ReleaseReservationRequest._();

  factory ReleaseReservationRequest([void updates(ReleaseReservationRequestBuilder b)]) = _$ReleaseReservationRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReleaseReservationRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReleaseReservationRequest> get serializer => _$ReleaseReservationRequestSerializer();
}

class _$ReleaseReservationRequestSerializer implements PrimitiveSerializer<ReleaseReservationRequest> {
  @override
  final Iterable<Type> types = const [ReleaseReservationRequest, _$ReleaseReservationRequest];

  @override
  final String wireName = r'ReleaseReservationRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReleaseReservationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.expectedVersion != null) {
      yield r'expectedVersion';
      yield serializers.serialize(
        object.expectedVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ReleaseReservationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReleaseReservationRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'expectedVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ReleaseReservationRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReleaseReservationRequestBuilder();
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


