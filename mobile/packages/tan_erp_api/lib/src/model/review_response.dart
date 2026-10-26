//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/qe_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'review_response.g.dart';

/// ReviewResponse
///
/// Properties:
/// * [id] 
/// * [sourceVersion] 
/// * [status] 
/// * [requestNote] 
/// * [requestedBy] 
/// * [requestedAtUtc] 
/// * [decidedBy] 
/// * [decidedAtUtc] 
/// * [reasonCode] 
/// * [decisionNote] 
@BuiltValue()
abstract class ReviewResponse implements Built<ReviewResponse, ReviewResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'sourceVersion')
  int? get sourceVersion;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'requestNote')
  String? get requestNote;

  @BuiltValueField(wireName: r'requestedBy')
  QePersonResponse? get requestedBy;

  @BuiltValueField(wireName: r'requestedAtUtc')
  DateTime? get requestedAtUtc;

  @BuiltValueField(wireName: r'decidedBy')
  QePersonResponse? get decidedBy;

  @BuiltValueField(wireName: r'decidedAtUtc')
  DateTime? get decidedAtUtc;

  @BuiltValueField(wireName: r'reasonCode')
  String? get reasonCode;

  @BuiltValueField(wireName: r'decisionNote')
  String? get decisionNote;

  ReviewResponse._();

  factory ReviewResponse([void updates(ReviewResponseBuilder b)]) = _$ReviewResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReviewResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReviewResponse> get serializer => _$ReviewResponseSerializer();
}

class _$ReviewResponseSerializer implements PrimitiveSerializer<ReviewResponse> {
  @override
  final Iterable<Type> types = const [ReviewResponse, _$ReviewResponse];

  @override
  final String wireName = r'ReviewResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReviewResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.sourceVersion != null) {
      yield r'sourceVersion';
      yield serializers.serialize(
        object.sourceVersion,
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
    if (object.requestNote != null) {
      yield r'requestNote';
      yield serializers.serialize(
        object.requestNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.requestedBy != null) {
      yield r'requestedBy';
      yield serializers.serialize(
        object.requestedBy,
        specifiedType: const FullType(QePersonResponse),
      );
    }
    if (object.requestedAtUtc != null) {
      yield r'requestedAtUtc';
      yield serializers.serialize(
        object.requestedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.decidedBy != null) {
      yield r'decidedBy';
      yield serializers.serialize(
        object.decidedBy,
        specifiedType: const FullType(QePersonResponse),
      );
    }
    if (object.decidedAtUtc != null) {
      yield r'decidedAtUtc';
      yield serializers.serialize(
        object.decidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.reasonCode != null) {
      yield r'reasonCode';
      yield serializers.serialize(
        object.reasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.decisionNote != null) {
      yield r'decisionNote';
      yield serializers.serialize(
        object.decisionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ReviewResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReviewResponseBuilder result,
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
        case r'sourceVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sourceVersion = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'requestNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.requestNote = valueDes;
          break;
        case r'requestedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QePersonResponse),
          ) as QePersonResponse?;
          if (valueDes == null) continue;
          result.requestedBy.replace(valueDes);
          break;
        case r'requestedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.requestedAtUtc = valueDes;
          break;
        case r'decidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QePersonResponse),
          ) as QePersonResponse?;
          if (valueDes == null) continue;
          result.decidedBy.replace(valueDes);
          break;
        case r'decidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.decidedAtUtc = valueDes;
          break;
        case r'reasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reasonCode = valueDes;
          break;
        case r'decisionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decisionNote = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ReviewResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReviewResponseBuilder();
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


