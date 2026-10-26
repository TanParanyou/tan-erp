//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'outbox_message_response.g.dart';

/// OutboxMessageResponse
///
/// Properties:
/// * [id] 
/// * [kind] 
/// * [resourceNumber] 
/// * [amount] 
/// * [status] 
/// * [attempts] 
/// * [nextAttemptAtUtc] 
/// * [lastError] 
/// * [externalRef] 
/// * [externalAmount] 
/// * [confirmedAtUtc] 
/// * [createdAtUtc] 
/// * [sentAtUtc] 
/// * [rowVersion] 
@BuiltValue()
abstract class OutboxMessageResponse implements Built<OutboxMessageResponse, OutboxMessageResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'resourceNumber')
  String? get resourceNumber;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'attempts')
  int? get attempts;

  @BuiltValueField(wireName: r'nextAttemptAtUtc')
  DateTime? get nextAttemptAtUtc;

  @BuiltValueField(wireName: r'lastError')
  String? get lastError;

  @BuiltValueField(wireName: r'externalRef')
  String? get externalRef;

  @BuiltValueField(wireName: r'externalAmount')
  double? get externalAmount;

  @BuiltValueField(wireName: r'confirmedAtUtc')
  DateTime? get confirmedAtUtc;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'sentAtUtc')
  DateTime? get sentAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  OutboxMessageResponse._();

  factory OutboxMessageResponse([void updates(OutboxMessageResponseBuilder b)]) = _$OutboxMessageResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OutboxMessageResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OutboxMessageResponse> get serializer => _$OutboxMessageResponseSerializer();
}

class _$OutboxMessageResponseSerializer implements PrimitiveSerializer<OutboxMessageResponse> {
  @override
  final Iterable<Type> types = const [OutboxMessageResponse, _$OutboxMessageResponse];

  @override
  final String wireName = r'OutboxMessageResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OutboxMessageResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.resourceNumber != null) {
      yield r'resourceNumber';
      yield serializers.serialize(
        object.resourceNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.attempts != null) {
      yield r'attempts';
      yield serializers.serialize(
        object.attempts,
        specifiedType: const FullType(int),
      );
    }
    if (object.nextAttemptAtUtc != null) {
      yield r'nextAttemptAtUtc';
      yield serializers.serialize(
        object.nextAttemptAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.lastError != null) {
      yield r'lastError';
      yield serializers.serialize(
        object.lastError,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.externalRef != null) {
      yield r'externalRef';
      yield serializers.serialize(
        object.externalRef,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.externalAmount != null) {
      yield r'externalAmount';
      yield serializers.serialize(
        object.externalAmount,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.confirmedAtUtc != null) {
      yield r'confirmedAtUtc';
      yield serializers.serialize(
        object.confirmedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.sentAtUtc != null) {
      yield r'sentAtUtc';
      yield serializers.serialize(
        object.sentAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OutboxMessageResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OutboxMessageResponseBuilder result,
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
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'resourceNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.resourceNumber = valueDes;
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'attempts':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.attempts = valueDes;
          break;
        case r'nextAttemptAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.nextAttemptAtUtc = valueDes;
          break;
        case r'lastError':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.lastError = valueDes;
          break;
        case r'externalRef':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.externalRef = valueDes;
          break;
        case r'externalAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.externalAmount = valueDes;
          break;
        case r'confirmedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.confirmedAtUtc = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'sentAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.sentAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  OutboxMessageResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OutboxMessageResponseBuilder();
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


