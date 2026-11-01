//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'billing_note_request.g.dart';

/// BillingNoteRequest
///
/// Properties:
/// * [noteType] 
/// * [reason] 
/// * [taxBaseDelta] 
@BuiltValue()
abstract class BillingNoteRequest implements Built<BillingNoteRequest, BillingNoteRequestBuilder> {
  @BuiltValueField(wireName: r'noteType')
  String? get noteType;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'taxBaseDelta')
  double? get taxBaseDelta;

  BillingNoteRequest._();

  factory BillingNoteRequest([void updates(BillingNoteRequestBuilder b)]) = _$BillingNoteRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BillingNoteRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BillingNoteRequest> get serializer => _$BillingNoteRequestSerializer();
}

class _$BillingNoteRequestSerializer implements PrimitiveSerializer<BillingNoteRequest> {
  @override
  final Iterable<Type> types = const [BillingNoteRequest, _$BillingNoteRequest];

  @override
  final String wireName = r'BillingNoteRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BillingNoteRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.noteType != null) {
      yield r'noteType';
      yield serializers.serialize(
        object.noteType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxBaseDelta != null) {
      yield r'taxBaseDelta';
      yield serializers.serialize(
        object.taxBaseDelta,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BillingNoteRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BillingNoteRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'noteType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.noteType = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'taxBaseDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.taxBaseDelta = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BillingNoteRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BillingNoteRequestBuilder();
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


